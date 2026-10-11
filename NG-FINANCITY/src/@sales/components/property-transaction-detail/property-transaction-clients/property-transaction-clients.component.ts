import { Component, OnInit, Input, ViewChild, Inject, OnDestroy } from '@angular/core';
import { ClientService } from 'src/@sales/services/client.service';
import { PGridIncellEditingComponent } from 'src/@common/components/p-grid-incell-editing/p-grid-incell-editing.component';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { ClientManagementDetailComponent } from '../../client-management-detail/client-management-detail.component';
import { BehaviorSubject } from 'rxjs';
import { Validators, FormGroup } from '@angular/forms';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PageUtilityService } from '../page.utility.service';
import { SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-property-transaction-clients',
  templateUrl: './property-transaction-clients.component.html',
  providers: [
    PWindowComponentService,
    ClientService,
  ]
})
export class PropertyTransactionClientsComponent implements OnDestroy, OnInit {

  @ViewChild('clientGrid', { read: PGridIncellEditingComponent, static: true }) public dataGrid: PGridIncellEditingComponent;
  @Input() formGroup: FormGroup;

  private _subsink = new SubSink();
  public clientDropDownSubject = new BehaviorSubject([]);

  constructor(public _windowService: PWindowComponentService,
    private _pageUtilityService: PageUtilityService,
    public _clientService: ClientService) {
  }

  ngOnInit(): void {

    this._initClient();
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  public _initClient() {

    this.dataGrid.config = {
      key: 'SysKey',
      fetch: this._pageUtilityService.clientSubject,
      columns: [
        { dataType: 'hidden', field: 'SysKey', defaultValue: null },
        { dataType: 'hidden', field: 'PTOSK', defaultValue: null },
        { dataType: 'hidden', field: 'TransType', defaultValue: null },
        {
          dataType: 'cascade_checkbox', field: 'IsMain', title: '', defaultValue: false, width: 50, cellValueChange: (c) => {

            if (c.event == true) {

              this.dataGrid.service.data.map((o, i) => {
                o['IsMain'] = false;
                o['TransType'] = 10;
              })
              c.fg.get('IsMain').setValue(true);
              c.fg.get('TransType').setValue(11);
            } else {
              c.fg.get('TransType').setValue(10);
            }
          }
        },
        {
          dataType: 'dropdown', field: 'ClientSK', title: 'Client',
          cellValueChange: (c) => {

            this._subsink.sink = this._clientService.findByKey(c.event)
              .subscribe(data => {

                c.fg.get('NRIC').setValue(data['Person']['NRIC']);
                c.fg.get('Blk').setValue(data['Contact']['BuildingHouseNo']);
                c.fg.get('Address').setValue(data['Contact']['Address1']);
                c.fg.get('Street').setValue(data['Contact']['Address2']);
                c.fg.get('FloorUnit').setValue(data['Contact']['FloorUnit']);
                c.fg.get('PostalCode').setValue(data['Contact']['PostalCode']);
                c.fg.get('Country').setValue(data['Contact']['Country']);
              });
          }
          , validators: Validators.required
          , dropdown: {
            fetch: this._clientService,
            textField: 'Name',
            valueField: 'SysKey',
            itemEmpty: (i) => this.addNewClient(i),
          },
        },
        { dataType: 'display', field: 'NRIC', title: 'NRIC No.', width: 120, align: 'center' },
        { dataType: 'display', field: 'Blk', title: 'Blk/House No.', width: 120, clickable: false },
        { dataType: 'display', field: 'Address', title: 'Address 1', width: 120, clickable: false },
        { dataType: 'display', field: 'Street', title: 'Address 2', width: 120, clickable: false },
        { dataType: 'display', field: 'FloorUnit', title: 'Floor-Unit', width: 120, clickable: false },
        { dataType: 'display', field: 'PostalCode', title: 'Postal Code', width: 120, clickable: false },
        { dataType: 'display', field: 'Country', title: 'Country', width: 120, clickable: false },
      ],
      rowDataBound: (b) => {

        this._subsink.sink = this._clientService.findByKey(b.data['ClientSK'])
          .subscribe(d => {

            b.data['NRIC'] = d['Person']['NRIC'];
            b.data['Blk'] = d['Contact']['BuildingHouseNo'];
            b.data['Address'] = d['Contact']['Address1'];
            b.data['Street'] = d['Contact']['Address2'];
            b.data['FloorUnit'] = d['Contact']['FloorUnit'];
            b.data['PostalCode'] = d['Contact']['PostalCode'];
            b.data['Country'] = d['Contact']['Country'];
          });
      }
    }
  }

  addNewClient(i?: (j: any) => any) {

    let state: WindowState = 'maximized';
    let window = this._windowService.open({ title: 'Client Management', componentType: ClientManagementDetailComponent, state: state });
    let windowComponent = window.content.instance as ClientManagementDetailComponent;

    windowComponent.loadData({ ClientName: this.dataGrid.dropDownFilterText });
    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(item => {
        if (i) {
          i(item.data.Person.SysKey);
        }
        this._windowService.close();
      });
  }
}
