FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:35792ea4ad1db051981f62b313f1be3b46b1f45cadbaa3c288cd0d3056eefb83 AS build-env
WORKDIR /App

COPY *.sln ./
COPY HackerNewsApi.csproj ./
RUN dotnet restore HackerNewsApi.csproj

COPY . ./
RUN dotnet publish HackerNewsApi.csproj -c Release -o /App/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:6c4df091e4e531bb93bdbfe7e7f0998e7ced344f54426b7e874116a3dc3233ff
WORKDIR /App
COPY --from=build-env /App/out .

RUN apt-get update && apt-get install -y curl net-tools

EXPOSE 5095

ENV ASPNETCORE_URLS=http://+:5095

ENTRYPOINT ["dotnet", "HackerNewsApi.dll"]
