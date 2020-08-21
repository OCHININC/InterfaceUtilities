#!/bin/bash

########################################
# printUsage						   #
########################################
function printUsage() {
  echo " ";
  if [ ! -z "$ERROR" ]
  then
    echo "Error: ${ERROR}";
    echo " "
  fi
  echo "Usage: start.sh [OPTION]... FILE..."
  echo "Example: start.sh -debug axolotl_order_rules.cfg"
  echo " "
  echo "Optional arguments:"
  echo -e "  -d, --debug \t\tsets the debug flag for the java process"
  echo -e "  --32 \t\tuse the 32 bit java executable"
  echo -e "  -v, --verbose \tbe verbose"
  echo -e "  --class=${CLASS_NAME},  \tfull class name to run"
  echo -e "  --mx=,  \tMax Heap in MB - number only 512 for 512M"
  
  echo " "

  exit
}
########################################

########################################
# setupDefaults						   #
########################################
function setupDefaults() {
  JAVA_DEBUG_MODE="false"
  JAVA_32BIT_MODE="true"
  VERBOSE_MODE="false"
  JAVA_CLASS="org.ochin.transport.gateway.GatewayGUM"
  HIX_USER="hixowner"
  MAX_HEAP="-Xmx64M"
  SEMAPHORES_DIR="/usr/local/ochin/InterfaceComm/run/semaphores"
}
########################################

########################################
# printEnv							   #
########################################

function printEnv() {
  echo "--------------------------------------"
  echo "java home: ${JAVA_HOME}"
  echo "java class: ${JAVA_CLASS}"
  echo "config file: ${CONFIG_FILE}"
  echo "app name: [${APP_NAME}]"
  echo "jmx port: [${JMX_PORT}]"
  echo "debug port: [${DEBUG_PORT}]"
  echo "Debug mode: ${JAVA_DEBUG_MODE}"
  echo "32bit mode: ${JAVA_32BIT_MODE}"
  echo "java command options: ${JAVA_COMMAND_OPTIONS}"
  echo "java -version:"
  echo " "
  ${JAVA_COMMAND} -version
  echo "--------------------------------------"
  echo " "
}

##################################################
# main
##################################################

# Setup some environment variables
# export JAVA_HOME=/usr/local/share/java
#export JAVA_HOME=/usr/lib/jvm/jdk1.7.0_06
export JAVA_HOME=/usr/lib/jvm/jre-1.8.0-openjdk
setupDefaults

# make sure all the log files have group write permissions
umask g+w

HIX_HOME='/usr/local/ochin/InterfaceComm'
# Source-in env vars that are not common - for example: timezone
. ${HIX_HOME}/hix.conf
cd /usr/local/ochin/InterfaceComm

if [ $# -eq 0 ]
 then
    ERROR="config file not specified - no args"
    printUsage
fi

while [ ! -z "$1" ]; do
	case "$1" in
		--help | -h)
			printUsage;
			;;
		--debug | -d)
			JAVA_DEBUG_MODE="true";
			;;
		--32)
			JAVA_32BIT_MODE="true";
			;;
		-v | --verbose)
			VERBOSE_MODE="true";
			;;
		--class=* )
			JAVA_CLASS=`echo $1 | cut -f2 -d'='`;
			;;
		-c )
			JAVA_CLASS=$2;
			shift; 
			;;
		--mx=* )
			MAX_HEAP_MEM=`echo $1 | cut -f2 -d'='`;
			MAX_HEAP="-Xmx${MAX_HEAP_MEM}M"
			;;
		*)
			break;
			;;
	esac

	shift
done

for CONFIG_FILE in $@; do
    if [ ! -r ${CONFIG_FILE} ]; then
	  echo "bad config file: ${CONFIG_FILE}"
	  exit
    fi 

    # setup the classpath
    CP=`find jars -name "*.jar"`
    CP=`echo . ${CP} | tr ' ' ':'`
    CP="${CP}:configs:resources"

    # CP=`ls   ./ochin_jars/*.jar ./jars/*.jar`
    # CP=`echo . ${CP} | tr ' ' ':'`

    APP_NAME=`grep appName ${CONFIG_FILE} | grep -v '#' | awk '{ split($0,a,"="); print a[2] }' | col | sed -e 's/ //g'`
    if [ -z ${APP_NAME} ]
    then
      echo "error: config file does not contain an appName"
      echo " "
      exit
    fi

    APP_CLASS=`grep appClass ${CONFIG_FILE} | grep -v '#' | awk '{ split($0,a,"="); print a[2] }' | col | sed -e 's/ //g'`
    if [ ! -z ${APP_CLASS} ]
    then
       JAVA_CLASS=${APP_CLASS}
    fi

    JMX_PORT=`grep jmx.port ${CONFIG_FILE} | grep -v '#' | awk '{ split($0,a,"="); print a[2] }' | col | sed -e 's/ //g'`

    DEBUG_PORT=$((${JMX_PORT} + 10000))

    JMX_OPTIONS="-Dcom.sun.management.jmxremote -Dcom.sun.management.jmxremote.port=${JMX_PORT} -Dcom.sun.management.jmxremote.authenticate='false' -Dcom.sun.management.jmxremote.ssl='false'"
    DEBUG_OPTIONS="-Xdebug -Xrunjdwp:transport=dt_socket,server=y,suspend=y,address=${DEBUG_PORT}"

    JAVA_COMMAND_OPTIONS="-D${APP_NAME}"
    if [ "${JAVA_DEBUG_MODE}" == "true" ]
    then
      JAVA_COMMAND_OPTIONS="${JAVA_COMMAND_OPTIONS} ${DEBUG_OPTIONS}"
    fi
    JAVA_COMMAND_OPTIONS="${JAVA_COMMAND_OPTIONS} ${JMX_OPTIONS} ${MAX_HEAP} -server -classpath ${CP}"

    if [ "${JAVA_32BIT_MODE}" == "true" ]
    then
      JAVA_COMMAND="${JAVA_HOME}/bin/java"
    else
      JAVA_COMMAND="${JAVA_HOME}/bin/sparcv9/java"
    fi

# before we execute the java, let's print out some env vars
    if [ "${VERBOSE_MODE}" == "true" ]
    then
      printEnv
      echo "starting GUM ..."
    fi

   user=`/usr/bin/env whoami`
   if [ "${user}" == "hixowner" ] 
   then
     ${JAVA_COMMAND} ${JAVA_COMMAND_OPTIONS} ${JAVA_CLASS} $CONFIG_FILE &
   else 
     sudo cat /dev/null
#    echo "sudo -u ${HIX_USER} -S ${JAVA_COMMAND} ${JAVA_COMMAND_OPTIONS} ${JAVA_CLASS} $CONFIG_FILE &"
     sudo -u ${HIX_USER} -S ${JAVA_COMMAND} ${JAVA_COMMAND_OPTIONS} ${JAVA_CLASS} $CONFIG_FILE &
   fi

    echo "GUM started ${APP_NAME} ${JAVA_CLASS} "
    echo " "
    touch ${SEMAPHORES_DIR}/${APP_NAME}
done
