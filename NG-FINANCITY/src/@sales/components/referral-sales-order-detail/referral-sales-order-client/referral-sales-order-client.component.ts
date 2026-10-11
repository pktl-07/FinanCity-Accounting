import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { ClientService } from 'src/@sales/services/client.service';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { ClientManagementDetailComponent } from '../../client-management-detail/client-management-detail.component';
import { FormGroup } from '@angular/forms';
import { mergeMap, map } from 'rxjs/operators';
import { WindowState } from '@progress/kendo-angular-dialog';
import { kendoFilter, SubSink } from 'src/@common/service/utility-service';
import { ContactService } from 'src/@sales/services/contact.service';

@Component({
  selector: 'app-referral-sales-order-client',
  templateUrl: './referral-sales-order-client.component.html',
  providers: [
    ClientService,
    PWindowComponentService,
    ContactService,
  ]
})
export class ReferralSalesOrderClientsComponent implements OnDestroy, OnInit {

  @Input() formGroup: FormGroup;
  private _subsink = new SubSink();

  constructor(public _windowService: PWindowComponentService,
    public _clientService: ClientService,
    public _contactService: ContactService) {
  }

  ngOnInit(): void {

    this._clientService.asyncFinds({
      filter: {
        logic: 'and',
        filters: kendoFilter([
          { field: 'SysKey', value: this.formGroup.get('Client').get('ClientSK').value, operator: 'eq', logic: 'and' }
        ])
      }
    })

    this._subsink.sink = this.formGroup.get('Client').get('ClientSK').valueChanges
      .subscribe(data => {

        this._subsink.sink = this._contactService.address(data)
          .pipe(
            map((response) => {

              const result = {};
              result['Address'] = response;
              return result;
            }),
            mergeMap((response) => {

              return this._clientService.findByKey(data)
                .pipe(map((client) => {

                  response['NRIC'] = client['Person']['NRIC']
                  return response;
                })
                )
            })
          )
          .subscribe((response) => {
            this.formGroup.get('Client').get('ClientAddress').patchValue(response['Address']);
            this.formGroup.get('Client').get('ClientNRIC').patchValue(response['NRIC']);
          })
      })
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  public itemDisabled(itemArgs: { dataItem: any, index: number }) {

    if (typeof itemArgs.dataItem.IsActive != 'undefined')
      return !itemArgs.dataItem.IsActive;

    return false;
  }

  private _filterText: string;
  handelClientFilterChange(clientsk: string) {

    this._filterText = clientsk;
    if (clientsk.length >= 3) {
      this._clientService.asyncFinds({
        filter: {
          logic: 'and',
          filters: kendoFilter([
            { field: 'Name', value: clientsk, operator: 'contains', logic: 'and' }
          ]),
        }
      })
    }
  }

  addNewClient() {

    let state: WindowState = 'maximized';
    let window = this._windowService.open({ title: 'Client Management', componentType: ClientManagementDetailComponent, state: state });
    let windowComponent = window.content.instance as ClientManagementDetailComponent;
    windowComponent.loadData({ ClientName: this._filterText });

    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(item => {

        this._clientService.asyncFinds({
          filter: {
            filters: kendoFilter([{ field: 'SysKey', value: item.data.Person.SysKey, operator: 'eq', logic: 'and' }]),
            logic: 'and'
          }
        });

        this.formGroup.get('Client').get('ClientSK').setValue(item.data.Person.SysKey);
        this._windowService.close();
      });
  }
}
