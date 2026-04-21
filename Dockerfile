FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY TeacherWorkplace.csproj ./
RUN dotnet restore TeacherWorkplace.csproj
COPY . ./
RUN dotnet publish TeacherWorkplace.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0
EXPOSE 8080
ENTRYPOINT ["dotnet", "TeacherWorkplace.dll"]