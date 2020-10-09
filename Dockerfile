FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["NetCoreUploadDemo.csproj", ""]
RUN dotnet restore "NetCoreUploadDemo.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "NetCoreUploadDemo.csproj" -c Release -o /app
ADD ./wwwroot/images /app/wwwroot/images

FROM build AS publish
RUN dotnet publish -o /app/ -c Release 

FROM base AS final
ENV GitHub=github.com

WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NetCoreUploadDemo.dll"]