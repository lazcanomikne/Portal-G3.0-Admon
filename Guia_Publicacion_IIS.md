# Guía de Publicación en IIS (Windows Server)

Esta guía detalla los pasos para desplegar la solución **PortalGovi** (Frontend Vue.js + Backend .NET Core) en un servidor IIS.

## Estrategia de Despliegue
Para simplificar la configuración y evitar problemas de **CORS**, se recomienda la siguiente estructura en el sitio de IIS:
*   **Raíz del Sitio (/)**: Servirá la aplicación Frontend (Vue.js).
*   **Sub-Aplicación (/api)**: Servirá el Backend (.NET Core).

De esta forma, ambas partes comparten el mismo dominio y puerto.

---

## 1. Prerrequisitos en el Servidor
1.  **IIS (Internet Information Services):** Asegúrate de que esten instalados los roles de Web Server.
2.  **Hosting Bundle .NET Core 3.1:** Descarga e instala el "Hosting Bundle" para .NET Core 3.1 desde el sitio oficial de Microsoft. Esto instala el *ASP.NET Core Module* necesario para IIS.
    *   *Nota:* Si instalas esto después de IIS, reinicia IIS ejecutando `iisreset` en CMD.
3.  **URL Rewrite Module:** Instala este módulo para IIS (necesario para el modo "History" de Vue.js).

---

## 2. Preparación del Backend (.NET Core)

1.  Abre una terminal en la carpeta del backend: `c:/PortalGovi2.0/Dispersion/PortalGovi`.
2.  Ejecuta el comando para publicar en versión Release:
    ```powershell
    dotnet publish -c Release -o c:/PortalGovi_Publicacion/api
    ```
    *(Puedes cambiar la ruta de salida `-o` a una carpeta temporal de tu elección)*.
3.  Verifica el archivo `appsettings.json` en la carpeta publicada y asegúrate de que la cadena de conexión a SQL Server apunte a la base de datos de producción.

---

## 3. Preparación del Frontend (Vue.js)

### Ajuste de URL Base
Antes de compilar, debemos asegurar que el Frontend llame al Backend usando rutas relativas (ya que estarán en el mismo dominio).

1.  Abre `src/main.js`.
2.  Busca la línea donde se define `url`.
3.  Cámbiala para que use una ruta relativa o detecte producción:
    ```javascript
    // src/main.js
    // En producción usará '' (ruta relativa), en desarrollo localhost:5000
    let url = process.env.NODE_ENV === 'production' ? '' : 'http://localhost:5000';
    ```
    *Al usar `''` (string vacío), axios hará peticiones a `/api/...` sobre el mismo dominio del sitio.*

### Compilación
1.  Abre una terminal en la carpeta del frontend: `c:/PortalGovi2.0/portal-govi`.
2.  Ejecuta el comando de construcción:
    ```powershell
    npm run build
    ```
3.  Esto generará una carpeta `dist`. Copia todo el contenido de `dist` a una carpeta temporal, por ejemplo: `c:/PortalGovi_Publicacion/frontend`.

---

## 4. Configuración en IIS

1.  **Crear Carpeta del Sitio:**
    *   Crea una carpeta en el servidor, ej: `C:\inetpub\wwwroot\PortalGovi`.
    *   Copia dentro el contenido del **Frontend** (lo que había en `dist`).
    *   Crea una subcarpeta llamada `api` dentro de `PortalGovi`.
    *   Copia dentro de `api` el contenido del **Backend** (lo que generó `dotnet publish`).

    Estructura final:
    ```text
    C:\inetpub\wwwroot\PortalGovi
    ├── CSS, JS, index.html, favicon.ico ... (Archivos Vue)
    ├── web.config (Para Vue Router, ver abajo)
    └── api
        ├── PortalGovi.exe, appsettings.json, web.config ... (Archivos .NET)
    ```

2.  **Crear App Pool:**
    *   En IIS Manager > Application Pools > Add Application Pool.
    *   Nombre: `PortalGoviPool`.
    *   .NET CLR Version: **No Managed Code** (Importante para .NET Core).
    *   Managed pipeline mode: Integrated.

3.  **Crear el Sitio Web:**
    *   Right click en "Sites" > Add Website.
    *   Site name: `PortalGovi`.
    *   Physical path: `C:\inetpub\wwwroot\PortalGovi`.
    *   Port: 80 (o el que desees).
    *   Host name: (Opcional, ej: portal.midominio.com).

4.  **Convertir Backend en Aplicación:**
    *   En IIS, despliega el sitio `PortalGovi`.
    *   Verás la carpeta `api`. Haz clic derecho sobre ella > **Convert to Application**.
    *   Select Application Pool: Elige `PortalGoviPool`.
    *   OK.

5.  **Configurar Vue Router (web.config para Frontend):**
    *   En la carpeta raíz (`C:\inetpub\wwwroot\PortalGovi`), crea (o edita) un archivo `web.config` con el siguiente contenido. Esto es necesario para que al refrescar la página (F5) en una ruta como `/inversion/apartados`, IIS no de error 404 y redirija al `index.html` de Vue.

    ```xml
    <?xml version="1.0" encoding="UTF-8"?>
    <configuration>
      <system.webServer>
        <rewrite>
          <rules>
            <rule name="Handle History Mode and custom 404/500" stopProcessing="true">
              <match url="(.*)" />
              <conditions logicalGrouping="MatchAll">
                <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
                <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
                <!-- Excluir la carpeta /api para que no la capture Vue -->
                <add input="{REQUEST_URI}" pattern="^/api/" negate="true" />
              </conditions>
              <action type="Rewrite" url="/" />
            </rule>
          </rules>
        </rewrite>
      </system.webServer>
    </configuration>
    ```

## 5. Verificación
1.  Navega a `http://localhost` (o tu dominio). Debería cargar el Login de Vue.
2.  Intenta hacer login. La petición irá a `http://localhost/api/Login...`.
3.  Si login funciona, ¡todo está listo!

---
**Solución de Problemas Comunes:**
*   **Error 500.19 en /api:** Revisa instalación del *Hosting Bundle*. Asegura que el AppPool sea "No Managed Code". Permissions de carpeta (IIS_IUSRS debe tener lectura).
*   **Error 404 en rutas al recargar:** Verifica que instalaste *URL Rewrite Module* y creaste el `web.config` en la raíz.
