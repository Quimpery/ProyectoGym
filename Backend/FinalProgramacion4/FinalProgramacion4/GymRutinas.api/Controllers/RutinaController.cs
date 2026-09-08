using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;

<? xml version = "1.0" encoding = "utf-8" ?>
< !--
  For more information on how to configure your ASP.NET application, please visit
  http://go.microsoft.com/fwlink/?LinkId=301879
  -->
< configuration >
  < configSections >
    < section name = "logoSuscriptor" type = "System.Configuration.NameValueSectionHandler" />
    < sectionGroup name = "poyectoCapasSettings" >
      < section name = "general" type = "WebGestionComercial.Configuracion.GeneralSection" />
      < section name = "formatos" type = "WebGestionComercial.Configuracion.FormatosSection" />
    </ sectionGroup >
    < !--For more information on Entity Framework configuration, visit http://go.microsoft.com/fwlink/?LinkID=237468 -->
    < section name = "entityFramework" type = "System.Data.Entity.Internal.ConfigFile.EntityFrameworkSection, EntityFramework, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission = "false" />
  </ configSections >
  < poyectoCapasSettings >
    < general lineaProductos = "Premium" enLaNube = "true" transactionIsolationLevel = "ReadUncommitted" />
    < formatos fechaCorta = "dd/MM/yyyy" precioUnitario = "0.000" importe = "0.00" cantidadArticulo = "0" porcentaje = "0.00%" />
  </ poyectoCapasSettings >
  < appSettings >
    < add key = "JWT_SECRET_KEY" value = "clave-secreta-api" />
    < add key = "JWT_AUDIENCE_TOKEN" value = "http://localhost:49220" />
    < add key = "JWT_ISSUER_TOKEN" value = "http://localhost:49220" />
    < add key = "JWT_EXPIRE_MINUTES" value = "21600" />
    < add key = "AUTHORIZATION_WHITE_LIST" value = "::1,127.0.0.1,190.7.59.138,190.7.59.133,190.7.60.170,10.47.64.103,190.183.58.79,181.29.190.56" />
    < add key = "AplicacionWebCoreUrl" value = "http://localhost:58070" />
    < add key = "tipoDeSoftware" value = "Gestión de ayudas económicas y servicios" />
    < add key = "linkWebGyA" value = "https://www.ertic.com" />
    < add key = "numeroLicencia" value = "yO/OurAyxzIyb6MBvsDDvw6BYmhi3CHrDVyJnGE2+GDB9uLAM8aFhCX3aVEUeklYZcLwMnuhMPc=" />
    < !--Fecha de vigencia hasta el 02/08/2050 -->
    <add key="fechacaducidad" value="x2oAG4lCED3j8RQrIDn6Pg==" />
    <add key="nombreEmpresa" value="GyAParana" />
    <add key="temaSistema" value="FEDEM" />
    <!-- Desactivada -->
    <add key="controlDeLicenciasPorUsuario" value="7Etmk+SR/LwpuIJD9FlJLg==" />
    <add key="tiempoDeExpiracionMinutos" value="120" />
    <add key="sucursal" value="12" />
    <add key="version" value="1.0.0" />
    <add key="BetaTestingProduccion" value="Produccion" />
    <add key="IntervaloProcesoBackground" value="600000" />
    <add key="nombreAplicacion" value="GestionCartera" />
    <add key="urlDemonio" value="http://localhost:8215" />
    <add key="RESTRINGIR_MENU_CARTERA_MOROSA" value="SI" />
    <add key="corsHabilitados" value="http://localhost:4200" />
    <add key="UsaArchivoDeConexiones" value="NO" />
  </appSettings>
  <logoSuscriptor>
    <add key="default" value="~/Content/Imagenes/logograndi.png" />
    <add key="lineabasegestionlite" value="~/Content/Imagenes/mas.png" />
  </logoSuscriptor>
  <!--
    For a description of web.config changes see http://go.microsoft.com/fwlink/?LinkId=235367.

    The following attributes can be set on the <httpRuntime> tag.
      <system.Web>
        <httpRuntime targetFramework="4.6.1" />
      </system.Web>
  -->
  <system.web>
    <globalization culture="es-AR" uiCulture="es-AR" requestEncoding="utf-8"
      responseEncoding="utf-8"
      fileEncoding="utf-8" />
    <compilation debug="true" targetFramework="4.8" />
    <httpRuntime targetFramework="4.8" maxRequestLength="2097152" />
  </system.web>
  <system.web.extensions>
    <scripting>
      <webServices>
        <jsonSerialization maxJsonLength="100000000" />
      </webServices>
    </scripting>
  </system.web.extensions>
  <system.webServer>
    <security>
      <requestFiltering>
        <requestLimits maxAllowedContentLength="2147483648" />
      </requestFiltering>
    </security>
    <validation validateIntegratedModeConfiguration="false" />
    <modules runAllManagedModulesForAllRequests="true">
      <remove name="WebDAVModule" />
    </modules>
    <httpProtocol>
      <customHeaders>
        <remove name="Access-Control-Allow-Credentials" />
        <add name="Access-Control-Allow-Credentials" value="true" />
        <remove name="Access-Control-Allow-Headers" />
        <add name="Access-Control-Allow-Headers" value="accept, cache-control, content-type, authorization" />
        <remove name="Access-Control-Allow-Methods" />
        <add name="Access-Control-Allow-Methods" value="GET, POST, DELETE, OPTIONS" />
      </customHeaders>
    </httpProtocol>
    <handlers>
      <remove name="ExtensionlessUrlHandler-Integrated-4.0" />
      <remove name="OPTIONSVerbHandler" />
      <remove name="TRACEVerbHandler" />
      <add name="ExtensionlessUrlHandler-Integrated-4.0" path="*." verb="*" type="System.Web.Handlers.TransferRequestHandler" preCondition="integratedMode,runtimeVersionv4.0" />
    </handlers>
  </system.webServer>
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="Castle.Windsor" publicKeyToken="407dd0808d44fbdc" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-4.0.0.0" newVersion="4.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Castle.Core" publicKeyToken="407dd0808d44fbdc" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-4.1.1.0" newVersion="4.1.1.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Newtonsoft.Json" publicKeyToken="30ad4fe6b2a6aeed" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-13.0.0.0" newVersion="13.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.Owin" publicKeyToken="31bf3856ad364e35" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-3.0.1.0" newVersion="3.0.1.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.Owin.Security" publicKeyToken="31bf3856ad364e35" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-3.0.1.0" newVersion="3.0.1.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.Owin.Security.Cookies" publicKeyToken="31bf3856ad364e35" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-3.0.1.0" newVersion="3.0.1.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.Bcl.AsyncInterfaces" publicKeyToken="cc7b13ffcd2ddd51" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-9.0.0.6" newVersion="9.0.0.6" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Memory" publicKeyToken="cc7b13ffcd2ddd51" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-4.0.1.2" newVersion="4.0.1.2" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.IdentityModel.Logging" publicKeyToken="31bf3856ad364e35" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-8.2.0.0" newVersion="8.2.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Runtime.CompilerServices.Unsafe" publicKeyToken="b03f5f7f11d50a3a" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-6.0.0.0" newVersion="6.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="WebGrease" publicKeyToken="31bf3856ad364e35" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-1.6.5135.21930" newVersion="1.6.5135.21930" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Antlr3.Runtime" publicKeyToken="eb42632606e9261f" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-3.5.0.2" newVersion="3.5.0.2" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="EnvDTE" publicKeyToken="B03F5F7F11D50A3A" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-8.0.0.0" newVersion="8.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Microsoft.SqlServer.Types" publicKeyToken="89845DCD8080CC91" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-11.0.0.0" newVersion="11.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="NLog" publicKeyToken="5120e14c03d0593c" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-4.0.0.0" newVersion="4.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Web.Optimization" publicKeyToken="31bf3856ad364e35" />
        <bindingRedirect oldVersion="1.0.0.0-1.1.0.0" newVersion="1.1.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Buffers" publicKeyToken="cc7b13ffcd2ddd51" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-4.0.3.0" newVersion="4.0.3.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Google.Protobuf" publicKeyToken="a7d26565bac4d604" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-3.31.1.0" newVersion="3.31.1.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="Google.Api.CommonProtos" publicKeyToken="3ec5ea7f18953e47" culture="neutral" />
        <bindingRedirect oldVersion="0.0.0.0-2.17.0.0" newVersion="2.17.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Web.Helpers" publicKeyToken="31bf3856ad364e35" />
        <bindingRedirect oldVersion="1.0.0.0-3.0.0.0" newVersion="3.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Web.WebPages" publicKeyToken="31bf3856ad364e35" />
        <bindingRedirect oldVersion="1.0.0.0-3.0.0.0" newVersion="3.0.0.0" />
      </dependentAssembly>
      <dependentAssembly>
        <assemblyIdentity name="System.Web.Mvc" publicKeyToken="31bf3856ad364e35" />
        <bindingRedirect oldVersion="0.0.0.0-5.2.4.0" newVersion="5.2.4.0" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
  <entityFramework>
    <defaultConnectionFactory type="System.Data.Entity.Infrastructure.LocalDbConnectionFactory, EntityFramework">
      <parameters>
        <parameter value="mssqllocaldb" />
      </parameters>
    </defaultConnectionFactory>
    <providers>
      <provider invariantName="System.Data.SqlClient" type="System.Data.Entity.SqlServer.SqlProviderServices, EntityFramework.SqlServer" />
    </providers>
  </entityFramework>
  <system.serviceModel>
    <bindings>
      <basicHttpBinding>
        <binding name="LoginCmsSoapBinding">
          <security mode="Transport" />
        </binding>
        <binding name="CTServicePortType">
          <security mode="Transport" />
        </binding>
        <binding name="ServiceSoap">
          <security mode="Transport" />
        </binding>
        <binding name="BasicHttpBinding_IServicioDialogFlow" />
        <binding name="BasicHttpsBinding_IServicioDialogFlow">
          <security mode="Transport" />
        </binding>
        <binding name="pushSoap" />
      </basicHttpBinding>
    </bindings>
    <client>
      <endpoint address="https://wsaa.afip.gov.ar/ws/services/LoginCms" binding="basicHttpBinding" bindingConfiguration="LoginCmsSoapBinding" contract="AfipWsaaProd.LoginCMS" name="LoginCms" />
      <endpoint address="https://servicios1.afip.gov.ar/wsfev1/service.asmx" binding="basicHttpBinding" bindingConfiguration="ServiceSoap" contract="AfipWsfevProd.ServiceSoap" name="ServiceSoap" />
      <endpoint address="https://wsaahomo.afip.gov.ar/ws/services/LoginCms" binding="basicHttpBinding" bindingConfiguration="LoginCmsSoapBinding" contract="AfipWsaa.LoginCMS" name="LoginCms" />
      <endpoint address="https://wswhomo.afip.gov.ar/wsfev1/service.asmx" binding="basicHttpBinding" bindingConfiguration="ServiceSoap" contract="AfipWsfev.ServiceSoap" name="ServiceSoap" />
      <endpoint address="http://pedidos.grandiyasociados.net/WebServices/ServicioDialogFlow/ServicioDialogFlow.svc" binding="basicHttpBinding" bindingConfiguration="BasicHttpBinding_IServicioDialogFlow" contract="ServicioDialogFlow.IServicioDialogFlow" name="BasicHttpBinding_IServicioDialogFlow" />
      <endpoint address="http://www.smscover.com/stringway/push.asmx" binding="basicHttpBinding" bindingConfiguration="pushSoap" contract="SmsPush.pushSoap" name="pushSoap" />
      <endpoint address="https://wswhomo.afip.gov.ar/wsctv1/service.asmx" binding="basicHttpBinding" bindingConfiguration="CTServicePortType" contract="AfipWsct.CTServicePortType" name="CTServicePortType" />
      <endpoint address="https://serviciosjava.afip.gob.ar/wsct/CTService" binding="basicHttpBinding" bindingConfiguration="CTServicePortType" contract="AfipWsctProd.CTServicePortType" name="CTServicePortType" />
    </client>
  </system.serviceModel>
</configuration>