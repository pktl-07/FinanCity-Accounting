import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from "./app.component";
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { PageLoaderModule } from 'src/@common/components/p-page-loader/p-page-loader.module';
import { IPageableConfig } from 'src/@common/Infrastructure/common-interface';
import { IGridConfig } from 'src/@common/Infrastructure/common-interface';
import { PToastModule } from 'src/@common/components/p-toast/p-toast-module';
import { GlobalConfigurationSubject, } from './mk-config';
import { AuthInterceptor } from 'src/@common/Infrastructure/auth.interceptor';
import { PMainMenuModule } from 'src/@common/components/p-main-menu/p-main-menu-module';
import { FCNotificationModule } from './components/notification/notificatoin.module';
//needs to import jquery eventhough we don't use the JQuery. 
//because app.component.ts have used SignalR object.
//declaring jquery import is a trick point to be enable to use SignalR object for debugging purpose.
import * as $ from 'jquery';

const pageableConfig: IPageableConfig = {
  buttonCount: 5,
  info: true,
  previousNext: true,
  type: 'numeric',
};

const gridConfig: IGridConfig = {
  pageSize: 14,
  scrollable: 'none',
  skip: 0,
  filterable: true,
}

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    PageLoaderModule,
    PToastModule,
    PMainMenuModule,
    HttpClientModule,
    FCNotificationModule,
  ],
  providers: [
    GlobalConfigurationSubject,
    { provide: 'pageable.config', useValue: pageableConfig },
    { provide: 'grid.config', useValue: gridConfig },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
