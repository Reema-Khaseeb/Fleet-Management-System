// import { ApplicationConfig } from '@angular/core';
// import { provideRouter } from '@angular/router';

// import { routes } from './app.routes';

// export const appConfig: ApplicationConfig = {
//   providers: [provideRouter(routes)]
// };
import { ApplicationConfig } from '@angular/core';
 import { provideRouter } from '@angular/router';

 import { routes } from './app.routes';
 import { provideClientHydration } from '@angular/platform-browser';
 import { provideHttpClient } from '@angular/common/http';

 export const appConfig: ApplicationConfig = {
   providers: [provideRouter(routes), 
    provideHttpClient()]
 };

 
//  export const appConfig: ApplicationConfig = {
//   providers: [provideRouter(routes), 
// provideClientHydration(),provideHttpClient()]
// };