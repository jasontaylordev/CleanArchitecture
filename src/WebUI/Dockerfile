# This stage is used for VS debugging on Docker
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
RUN apt-get update && apt-get install -y libxml2
ENV ASPNETCORE_URLS=https://+:5001;http://+:5000
ENV PATH="/app/bin/Debug/net6.0/clidriver/bin:/app/bin/Debug/net6.0/clidriver/lib/:${PATH}"
ENV LD_LIBRARY_PATH="/app/bin/Debug/net6.0/clidriver/lib/:/app/bin/Debug/net6.0/clidriver/lib/icc"
ENV DB2_CLI_DRIVER_INSTALL_PATH="/app/bin/Debug/net6.0/clidriver"
ENV DB2LICENSE_PATH="/db2/license/db2consv_ee.lic"
ARG DB2LICENSE
RUN mkdir -p /db2/license
RUN mkdir -p $DB2_CLI_DRIVER_INSTALL_PATH/license
RUN echo $DB2LICENSE | base64 -d > $DB2LICENSE_PATH
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/WebUI/WebUI.csproj", "src/WebUI/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/WebUI/WebUI.csproj"
COPY . .
WORKDIR "/src/src/WebUI"
RUN dotnet build "WebUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebUI.csproj" -c Release -o /app/publish

FROM base AS final
ENV PATH="/app/clidriver/bin:/app/clidriver/lib/:${PATH}"
ENV LD_LIBRARY_PATH="/app/clidriver/lib:/app/clidriver/lib/icc"
ENV DB2_CLI_DRIVER_INSTALL_PATH="/app/clidriver"
WORKDIR /app
COPY --from=publish /app/publish .
RUN cp /db2/license/db2consv_ee.lic /app/clidriver/license
ENTRYPOINT ["dotnet", "CleanArchitecture.WebUI.dll"]