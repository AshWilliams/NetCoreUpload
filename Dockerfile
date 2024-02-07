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
ENV Ambiente=docker
ENV correo=robert.rozas.n@outlook.com
ENV Logo=https://preview.redd.it/erdjqzlmlv111.png?auto=webp&s=1dfcc14e586e2aaa0ee7fb9a1bb9da6679ec892b

WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "NetCoreUploadDemo.dll"]