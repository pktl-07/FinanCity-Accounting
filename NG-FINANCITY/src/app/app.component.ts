import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, Event, NavigationStart, NavigationEnd, NavigationCancel, NavigationError } from '@angular/router';
import { PageLoaderService } from '../@common/components/p-page-loader/p-page-loader.service';
import { UserAccountService } from 'src/@user-accounts/services/user-account.service';
import { GlobalConfigurationSubject } from './mk-config';
import { NotificationService } from 'src/@sales/services/notification-service';
import { SubSink } from 'src/@common/service/utility-service';
import { environment } from 'src/environments/environment';

declare var $: any;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  providers: [
    UserAccountService,
    NotificationService,
  ]
})
export class AppComponent implements OnInit, OnDestroy {

  private _subsink = new SubSink();
  private _signalRConnection: any;

  constructor(public _router: Router,
    private _userAccountService: UserAccountService,
    public _global: GlobalConfigurationSubject,
    private _notification: NotificationService,
    public _pageLoaderService: PageLoaderService) {

    this._subsink.sink = this._router.events
      .subscribe((routerEvent: Event) => {

        if (routerEvent instanceof NavigationStart) {
          this._pageLoaderService.loading = true;
        }

        if (routerEvent instanceof NavigationEnd ||
          routerEvent instanceof NavigationCancel ||
          routerEvent instanceof NavigationError) {

          this._pageLoaderService.loading = false;
        }
      })
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  ngOnInit(): void {

    this.subscribeGlobleConfig();
    this.subscribeSignalR();
  }

  subscribeGlobleConfig() {

    this._subsink.sink = this._global.FinanCityConfig
      .subscribe(d => {

        if (d.auth.isAuthenticate == false)
          this._router.navigateByUrl("/Login");

      })
  }

  proxy: any;
  subscribeSignalR() {

    this._subsink.sink = this._global.FinanCityConfig
      .subscribe(config => {

        if (config.auth.isAuthenticate == true) {

          this._signalRConnection = $.hubConnection(`${environment.auth.api}/signalr`);
          this.proxy = this._signalRConnection.createHubProxy('FCHub');

          this.proxy.on('NotifyOnChattingChange', () => {
            this._notification.getChattingCount();
          })

          this.proxy.on('NotifyOnSalesMortgageChange', () => {

            console.log('dostaff')
            this._notification.getMortgageSalesCount();
          })

          this._signalRConnection.start()
            .done(() => {

              this._notification.getChattingCount();
              this._notification.getMortgageSalesCount();

              this._signalRConnection.stateChanged((state: any) => {
                if (state.newState == $.signalR.connectionState.disconnected) {
                  this._notification.wipeSignalR();
                }
              })
            })
        }
      })
  }

  signOut() {

    this._userAccountService.SignOut().then(() => {
      this._signalRConnection.stop();
    })
  }
}
