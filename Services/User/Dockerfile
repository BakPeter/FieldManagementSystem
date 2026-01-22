FROM rt-srv:443/docker/alpine:3.19.0

RUN rm /etc/apk/repositories && \
    apk update && \
    echo 'http://rt-srv.aeronautics-sys.com/artifactory/alpine/v3.19/main' >> /etc/apk/repositories && \
    echo 'http://rt-srv.aeronautics-sys.com/artifactory/alpine/v3.19/community' >> /etc/apk/repositories  && \
    cat /etc/apk/repositories

RUN apk add --allow-untrusted --no-cache openssh libstdc++ libintl libc6-compat icu

WORKDIR /app
COPY . .

RUN chmod 777 ./GenericGateway

EXPOSE 8080

VOLUME [ "./data" ]

# ENTRYPOINT ["./GenericGateway", "-s", "${GGW_PROCESS}"]
ENTRYPOINT ["/bin/sh", "-c", "/app/GenericGateway -s $GGW_PROCESS \"$@\""]
#ENTRYPOINT ["echo $GGW_PROCESS_CONFIGURATION_NAME"]
# ENTRYPOINT ["tail", "-f", "/dev/null"]
# ENTRYPOINT ["./GenericGateway.exe", "-s", "proccess-telemetry-kafka.json "]