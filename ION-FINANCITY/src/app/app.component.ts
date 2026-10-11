import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { Platform, ToastController, AlertController } from '@ionic/angular';
import { SplashScreen } from '@ionic-native/splash-screen/ngx';
import { StatusBar } from '@ionic-native/status-bar/ngx';
import { Router, RouterEvent, NavigationStart, NavigationEnd, NavigationCancel, NavigationError } from '@angular/router';
import { INavigateMenuItem, IFCConfig, logoPicture, SubSink } from 'src/@common/utils';
import { AuthService } from './services/auth-service';
import { BehaviorSubject } from 'rxjs';
import { AppMenuService } from '../app/services/app-menu-service';
import { FCNetworkService, ConnectionStatus } from 'src/@common/components/native-devices/fc-network.service';
import { NotificationService } from './services/notification.service';
import { AppVersion } from '@ionic-native/app-version/ngx';
import { SQLiteService } from './services/sqlite.service';
import { environment } from 'src/environments/environment';

declare var $: any;

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss'],
  providers: [
    FCNetworkService,
    AppVersion,
    AuthService,
    NotificationService,
  ]
})
export class AppComponent implements OnInit, OnDestroy {

  public appPages: Array<INavigateMenuItem> = new Array<any>();
  public selectedPath: string;
  public loadingState = new BehaviorSubject(false);
  private _subsink = new SubSink();
  private _authClone: { url: string, spsk: string };

  public config: IFCConfig = environment;

  constructor(
    private _platform: Platform,
    private _router: Router,
    private _auth: AuthService,
    appMenService: AppMenuService,
    private _appVersion: AppVersion,
    private _alertController: AlertController,
    private _notification: NotificationService,
    private _sqlLiteService: SQLiteService,
    private _toastController: ToastController,
    private _splashScreen: SplashScreen,
    private _netWorkService: FCNetworkService,
    private _statusBar: StatusBar) {

    this.appPages = appMenService.appPages;
    this.initializeApp();
  }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  initializeApp() {

    this._platform.ready().then(() => {
      this._statusBar.styleDefault();
      this._splashScreen.hide();
      this.subscribeNetWorkStatus();
      this.subscribeRouterEvent();
      this.subscribeAuthenticationState();

      if (this._platform.is('mobile')) {
        this._sqlLiteService.createDb().then(() => {
          this._sqlLiteService.createTable().then(() => {
            environment.dbReady.next(true);
          }).catch((error) => alert(error))
        }).catch((error) => alert(error))
      }

      this._subsink.sink = environment.general.version
        .subscribe(async (version) => {
          if (await this.compareVersion(version))
            await this.setVersionAlert();
        })
    })
  }

  getPicture(key: string) {
    return logoPicture[key];
  }

  private subscribeRouterEvent() {

    this._router.events.subscribe((event: RouterEvent) => {

      if (event && event.url)
        this.selectedPath = event.url;

      if (event instanceof NavigationStart)
        this.loadingState.next(true);

      if (event instanceof NavigationEnd ||
        event instanceof NavigationCancel ||
        event instanceof NavigationError) {

        this.loadingState.next(false);
      }
    })
  }

  private subscribeAuthenticationState() {

    this._subsink.sink = environment.auth.isAuthenticate.subscribe((value) => {
      if (value) {

        this._router.navigate(['home']);
        //should subscribe signalr object when user has already login.
        //
        this.subscribeSignalR();
      }
      else
        this._router.navigate(['login']);
    })
  }

  private subscribeNetWorkStatus() {

    if (this._platform.is('cordova')) {
      this._subsink.sink = this._netWorkService.onNetworkChange()
        .subscribe((status) => {
          if (status == ConnectionStatus.Offline) {
            const toast = this._toastController.create({
              message: `Network connection failed, please check the network.`,
              duration: 3000,
              position: "bottom"
            });
            toast.then(toast => toast.present());
          }
        })
    }
  }

  private _signalRConnection: any;
  private subscribeSignalR() {

    this._signalRConnection = $.hubConnection(`${environment.auth.api}/signalr`);
    const proxy = this._signalRConnection.createHubProxy('FCHub');

    proxy.on('NotifyOnChattingChange', () => {
      this._notification.subscribeChattingCount();
    })

    proxy.on('NotifyOnSalesMortgageChange', () => {
      this._notification.subscribeMortgageSalesCount();
    })

    proxy.on('NotifyOnLeadFollowUp', () => {
      this._notification.subscribeLeadFollowUpCount();
    })

    if (this._platform.is('android')) {
      proxy.on('NotifyOnAndriodVersionChange', (version) => {
        environment.general.version.next(version);
      })
    }

    if (this._platform.is('ios')) {
      proxy.on('NotifyOnIOSVersionChange', (version) => {
        environment.general.version.next(version);
      })
    }

    this._signalRConnection.start()
      .done(() => {

        this._notification.subscribeChattingCount();
        this._notification.subscribeMortgageSalesCount();
        this._notification.subscribeLeadFollowUpCount();

        if (this._platform.is('ios'))
          this._notification.subscribeIOSVersion()

        if (this._platform.is('android'))
          this._notification.subScribeAndriodVersion()

        this._signalRConnection.stateChanged((state: any) => {
          if (state.newState == $.signalR.connectionState.disconnected) {
            this._notification.wipeSignalR(this._authClone);
          }
        })
      })
  }

  onLogoutClick() {

    this._authClone = {
      url: environment.auth.api,
      spsk: environment.auth.SPSK
    }

    this._auth.Logout().then(() => {
      this._signalRConnection.stop();
    })
  }

  async setVersionAlert() {

    const a = await this._alertController.create({
      header: 'Version old.',
      message: `Sorry, this version of FinanCity is no longer to support. Please upgrade to a new version.`,
      buttons: [{
        text: 'OK', handler: () => {
          navigator['app'].exitApp()
        }
      }]
    })

    await a.present();
  }

  compareVersion(version: number): Promise<boolean> {

    return new Promise(async (resolve) => {
      const app = await this._appVersion.getVersionNumber();
      resolve(Number(app) < version)
    })
  }
}
