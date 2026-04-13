#Build stage
#https://stackoverflow.com/questions/61108021/gradle-and-docker-how-to-run-a-gradle-build-within-docker-container

FROM gradle:9.4.1-jdk17 AS BUILD
WORKDIR /usr/app/
COPY . . 
RUN gradle build

# Package stage

FROM amazoncorretto:17-alpine
ENV JAR_NAME=app.jar
ENV APP_HOME=/usr/app/
WORKDIR $APP_HOME
COPY --from=build /usr/app/build/libs/*.jar app.jar
Expose 8080
ENTRYPOINT ["java", "-jar", "app.jar"]