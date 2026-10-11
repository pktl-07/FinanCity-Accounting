import { NgModule, Injector } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { IonicModule } from '@ionic/angular';
import { SplashScreen } from '@ionic-native/splash-screen/ngx';
import { StatusBar } from '@ionic-native/status-bar/ngx';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from 'src/@common/Infrastructure/auth.interceptor';
import { IonicStorageModule } from '@ionic/storage';
import { SuperTabsModule } from '@ionic-super-tabs/angular'
import { File } from '@ionic-native/file/ngx';
import { FaConfig, FaIconLibrary, FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { fas } from '@fortawesome/pro-solid-svg-icons'
import { far } from '@fortawesome/pro-regular-svg-icons'
import { fab } from '@fortawesome/free-brands-svg-icons'
import { FilePath } from '@ionic-native/file-path/ngx';
import { Camera } from '@ionic-native/camera/ngx';
//needs to import jquery eventhough we don't use the JQuery. 
//because app.component.ts have used SignalR object.
//declaring jquery import is a trick point to be enable to use SignalR object for debugging purpose.
import { FileOpener } from '@ionic-native/file-opener/ngx';
import { Network } from '@ionic-native/network/ngx';
import { GooglePlus } from '@ionic-native/google-plus/ngx';
import { Facebook } from '@ionic-native/facebook/ngx';
import { SignInWithApple } from '@ionic-native/sign-in-with-apple/ngx';
import { SQLite } from '@ionic-native/sqlite/ngx';
import { SQLiteService } from './services/sqlite.service';

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    BrowserModule,
    IonicModule.forRoot(),
    FontAwesomeModule,
    AppRoutingModule,
    HttpClientModule,
    IonicStorageModule.forRoot(),
    SuperTabsModule.forRoot(),

  ],
  providers: [
    FaIconLibrary,
    FaConfig,
    StatusBar,
    SplashScreen,
    FileOpener,
    Storage,
    SQLite,
    SQLiteService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
    FilePath,
    File,
    Camera,
    Network,
    GooglePlus,
    Facebook,
    SignInWithApple
  ],
  bootstrap: [AppComponent],

})
export class AppModule {

  static injector: Injector;

  constructor(private injector: Injector,
    private _library: FaIconLibrary) {
    _library.addIconPacks(fas, far, fab)
    AppModule.injector = injector;
  }
}
