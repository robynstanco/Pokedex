# Pokédex
<div>
  <h2>ASP.NET Core 3.1 MVC/Web API Application.</h2>
  <h3><a href="https://mypokedex.azurewebsites.net/" target="_blank">MyPokedex [Hosted on Azure App Services]</a></h3>
  <p>Search National Dex Pokémon based on a variety of criterion. Create, modify, and view a personal Pokédex.</p>
</div>
<div style="display: flex;">
  <img src="https://github.com/robynstanco/Pokedex/workflows/Build,%20test,%20and%20Deploy%20ASP.Net%20Core%20app%20to%20Azure%20Web%20App%20-%20mypokedex/badge.svg?branch=master"/>
  <img src="https://img.shields.io/github/last-commit/robynstanco/pokedex/master?logo=github"/>
  <img src="https://img.shields.io/github/languages/code-size/robynstanco/Pokedex?logo=github"/>
  <img src="https://img.shields.io/github/repo-size/robynstanco/Pokedex?logo=github"/>
  <img src="https://img.shields.io/github/issues/robynstanco/Pokedex?logo=github"/>
  <img src="https://img.shields.io/github/issues-closed/robynstanco/Pokedex?logo=github"/>
  <img src="https://img.shields.io/github/issues-pr-closed/robynstanco/pokedex?logo=github"/>
  <img src="https://img.shields.io/github/stars/robynstanco/Pokedex?logo=github"/>
  <img src="https://img.shields.io/github/languages/top/robynstanco/Pokedex?logo=github"/>
</div>
<div>
  <h3>Technologies & Methodologies</h3>
  <ul>
    <li>ASP.NET Core MVC & Web API</li>
    <li>SQL Server, SSMS, T-SQL</li>
    <li>Cloudscribe Models/Pagination</li>
    <li>Repository Pattern</li>
    <li>EntityFrameworkCore</li>
    <li>LINQ</li>
    <li>Moq & MSTest Frameworks</li>
    <li>JavaScript, jQuery, HTML5, LESS, CSS3, WebCompiler, BEM Notation, Minification</li>
    <li>Swagger & Postman</li>
    <li>Azure App Services</li>
    <li>Azure SQL Database</li>
    <li>Azure Application Insights</li>
    <li>Visual Studio</li>
  </ul>
  <h3>Local Setup</h3>
  <ol>
    <li>
      <span>Prerequisites</span>
      <ul>
        <li>Visual Studio</li>
        <li>SQLServer Instance</li>
        <li>SSMS & SSDT</li>
        <li>Web Compiler [used for LESS compilation, only needed if you make changes]</li>
        <li>Postman</li>
      </ul>
    </li>
    <li>Publish POKEDEXDB to localdb.</li>
    <li>Update the PokedexApp/PokedexAPI appsettings.Development.json connection string to point to the newly create database.</li>
    <li>Ensure nuget packages are restored and ready to use.</li>
    <li>Build, Run Tests, Run Application locally. Note: if calling API, utilize Postman Collection.</li>
  </ol>
  <p>Note: see repository <a href="https://github.com/robynstanco/Pokedex/wiki">Wiki</a> for detailed Architecture, Design, and Implementation</p> 
</div>
