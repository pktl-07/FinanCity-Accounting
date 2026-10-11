import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { UserAccountService } from 'src/@user-accounts/services/user-account.service';
import { WindowState } from '@progress/kendo-angular-dialog';
import { UserManagementDetailComponent } from '../user-management-detail/user-management-detail.component';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-user-management-list',
  templateUrl: './user-management-list.component.html',
  providers: [
    UserAccountService,
    PWindowComponentService
  ]
})
export class UserManagementListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true })
  _toolbar: PToolbarComponent;

  @ViewChild(PGridViewComponent, { static: true })
  _gridView: PGridViewComponent;

  public _unsubscribe: Subject<void> = new Subject();

  constructor(public _router: Router,
    public _windowService: PWindowComponentService,
    public _userAccountService: UserAccountService) {
  }

  ngOnInit(): void {

    this._userAccountService.pipe(takeUntil(this._unsubscribe)).subscribe();

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => this.createNew() });

    this._gridView.config = {
      key: 'SysKey',
      methodName: 'getPager',
      filterable: 'window',
      fetch: this._userAccountService,
      columns: [
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string) => this.createNew(a) },
        { dataType: 'string', field: 'PasswordLoginUserName', filter: true, title: 'User Name' },
        { dataType: 'string', field: 'Google', filter: true, title: 'Google' },
        { dataType: 'string', field: 'Facebook', filter: true, title: 'Facebook' },
        { dataType: 'string', field: 'FirstName', filter: true, title: 'Frist Name' },
        { dataType: 'string', field: 'LastName', filter: true, title: 'Last Name' },
        {
          dataType: 'string', field: 'AccountType', filter: true, title: 'Account Type', filterOptions: {
            control: 'multi-select',
            source: [{ text: 'Admin', value: 1 }, { text: 'Salesperson', value: 2 }],
          }
        },
        { dataType: 'checkbox', field: 'IsActive', filter: true, title: 'Is Active' },
      ], rowDataBound: (data => {

        data.data['AccountType'] = data.data['AccountType'] == 1 ? 'Admin' : 'Salesperson';
      })
    }
  }

  ngOnDestroy(): void {

    this._unsubscribe.complete();
    this._unsubscribe.next();
  }

  createNew(key = null): Promise<any> {

    return new Promise<any>((resolve) => {

      let state: WindowState = 'default';
      let window = this._windowService.open({ title: 'User Management', componentType: UserManagementDetailComponent, state: state });
      let windowComponent = window.content.instance as UserManagementDetailComponent;
      windowComponent.loadData(key);

      windowComponent.OnSuccess.pipe(takeUntil(this._unsubscribe))
        .subscribe(() => {
          this._gridView.refresh();
        });

      resolve(null);
    })
  }
}
