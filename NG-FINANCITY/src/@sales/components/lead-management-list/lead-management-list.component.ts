import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { Router } from '@angular/router';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { LeadService } from 'src/@sales/services/lead.service';
import { LeadManagementDetailComponent } from '../lead-management-detail/lead-management-detail.component';
import { LeadCloseRate, SubSink } from 'src/@common/service/utility-service';
import { ChattingRemarkService } from 'src/@sales/services/chatting-remark.service';

@Component({
  selector: 'app-lead-management-list',
  templateUrl: './lead-management-list.component.html',
  providers: [LeadService,
    ChattingRemarkService,
    PWindowComponentService]
})
export class LeadManagementListComponent implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true })
  _toolbar: PToolbarComponent;

  @ViewChild(PGridViewComponent, { static: true })
  _gridView: PGridViewComponent;

  private _subsink = new SubSink();

  constructor(
    private _chattingService: ChattingRemarkService,
    public _windowService: PWindowComponentService,
    public _leadService: LeadService,
    public _router: Router) {
  }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => this._router.navigateByUrl('') })
      .NavigateNew({ action: () => this.createNew({}) });

    this._gridView.config = {
      key: 'SysKey',
      methodName: 'getPager',
      fetch: this._leadService,
      filterable: 'window',
      // detailTemplate: LeadManagementSalespersonListComponent,
      columns: [
        { dataType: 'button', field: 'SysKey', filter: false, title: '', viewDetail: (a: string) => this.createNew({ SysKey: a }) },
        { dataType: 'string', field: 'LeadNo', filter: true, title: 'Client No.', width: 120 },
        { dataType: 'string', field: 'Name', filter: true, title: 'Client Name' },
        { dataType: 'string', field: 'SalespersonName', filter: true, width: 120, title: 'Salesperson' },
        { dataType: 'checkbox', field: 'IsOrganization', filter: true, title: 'Is Organization' },
        // { dataType: 'string', field: 'NRIC', filter: true, title: 'NRIC/ACRA No.', width: 120 },
        {
          dataType: 'string', field: 'LeadCloseRate', filter: true, title: 'Lead Close Rate', width: 100,
          filterOptions: {
            control: 'dropdown',
            source: LeadCloseRate,
            textField: 'text',
            valueField: 'value'
          }
        },
        { dataType: 'string', field: 'SalesActivity', filter: false, title: 'Sales Activity', width: 150 },
        { dataType: 'string', field: 'LastRemark', filter: false, title: 'Last: Remark' },
        { dataType: 'date', field: 'FollowupDate', filter: false, title: 'Follow-Up date', width: 150 },
        { dataType: 'string', field: 'Mobile', filter: true, title: 'Mobile', width: 100 },
        { dataType: 'string', field: 'Email', filter: true, title: 'Email', width: 100 },
        { dataType: 'checkbox', field: 'IsActive', filter: true, title: 'Is Active' },
      ],
      rowDataBound: (data) => {

        this._subsink.sink = this._chattingService.last(data.data['SysKey'])
          .subscribe((message: any) => {

            if (message && message != null)
              data.data['LastRemark'] = message['Message']
          })
        data.data['LeadCloseRate'] = LeadCloseRate.find(x => x.value == data.data['LeadCloseRate']).text
      }
    }
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  createNew(param: { SysKey?: string }): Promise<any> {

    return new Promise<any>((resolve) => {

      let state: WindowState = 'maximized';
      let window = this._windowService.open({ title: 'Lead Management', componentType: LeadManagementDetailComponent, state: state });
      let windowComponent = window.content.instance as LeadManagementDetailComponent;

      windowComponent.loadData({ SysKey: param.SysKey });
      this._subsink.sink = windowComponent.OnSuccess
        .subscribe(() => {
          this._gridView.refresh();
        });
      resolve(null);
    })
  }
}
