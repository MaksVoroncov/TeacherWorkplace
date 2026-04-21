FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY TeacherWorkplace/TeacherWorkplace.csproj ./TeacherWorkplace/
RUN dotnet restore TeacherWorkplace/TeacherWorkplace.csproj
COPY TeacherWorkplace/ ./TeacherWorkplace/
RUN dotnet publish TeacherWorkplace/TeacherWorkplace.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=0
EXPOSE 8080
ENTRYPOINT ["dotnet", "TeacherWorkplace.dll"]