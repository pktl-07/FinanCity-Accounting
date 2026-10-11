import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { VendorService } from '../../../services/vendor.service';
import { SalespersonManagementService } from '../../../services/salesperson-management.service';
import { PWindowComponentService } from '../../../../@common/components/p-window/p-window.component.service';
import { WindowState } from '@progress/kendo-angular-dialog';
import { VendorManagementDetailComponent } from '../../vendor-management-detail/vendor-management-detail.component';
import { ContactService } from 'src/@sales/services/contact.service';
import { vendorType, kendoFilter, SubSink } from 'src/@common/service/utility-service';
import { ExternalSalespersonDetailComponent } from '../../external-salesperson-detail/external-salesperson-detail.component';

@Component({
  selector: 'app-property-transaction-billtocobroke',
  templateUrl: './property-transaction-billtocobroke.component.html',
  providers: [
    VendorService,
    SalespersonManagementService,
    ContactService,
    PWindowComponentService
  ]
})
export class PropertyTransactionBillToCobrokeComponent implements OnDestroy, OnInit {

  @Input() formGroup: FormGroup;
  @Input() readonlyFields: boolean = false;

  public _mortgagerContactFormGroup: FormGroup = new FormGroup({});
  private _subsink = new SubSink()
  public _vendorType = [];

  get readonly() {
    return this.formGroup.get('RecordStatus').value >= 6;
  }

  companyDefaultItem: any = { SysKey: null, CompanyName: 'N/A' };
  bankerDefaultItem: any = { SysKey: null, Name: 'N/A' };
  clientTraansTypeDefaultItem: any = { value: null, text: 'N/A' };

  disableVendor = false;
  disableContact = false;

  constructor(
    public _vendorService: VendorService,
    public _contactService: ContactService,
    public _peopleService: SalespersonManagementService,
    public _windowService: PWindowComponentService,
  ) { }

  ngOnInit(): void {

    this.formGroup.addControl('VendorSK', new FormControl());
    this.formGroup.addControl('BuildingHouseNo', new FormControl());
    this.formGroup.addControl('Address1', new FormControl());
    this.formGroup.addControl('Address2', new FormControl());
    this.formGroup.addControl('FloorUnit', new FormControl());
    this.formGroup.addControl('PostalCode', new FormControl());
    this.formGroup.addControl('Country', new FormControl());

    this._vendorType = vendorType();

    this._vendorService.asyncFinds({
      filter: {
        logic: 'and',
        filters: kendoFilter([{ field: 'TransType', value: this.formGroup.get('ClientTransType').value, operator: 'eq', logic: 'and' }])
      }
    });

    //'Vendor Type' change event
    this._subsink.sink = this.formGroup.get('ClientTransType').valueChanges
      .subscribe(item => {

        this.formGroup.get('VendorSK').patchValue(null);

        if (item == null) {
          this.formGroup.get('VendorSK').disable({ emitEvent: false });
          this.disableVendor = true;
        }
        else {
          this.formGroup.get('VendorSK').enable({ emitEvent: false });
          this.disableVendor = false;
          this._vendorService.asyncFinds({
            filter: {
              filters: kendoFilter([
                { field: 'TransType', value: item, operator: 'eq', logic: 'and' }
              ]),
              logic: 'and'
            }
          });
        }
      })

    //'Company Name' change event.
    this._subsink.sink = this.formGroup.get('VendorSK').valueChanges
      .subscribe(item => {

        this.formGroup.get('BankerSK').patchValue(null);

        if (item == null) {
          this.formGroup.get('BankerSK').disable();
          this.disableContact = true;
        } else {

          this.formGroup.get('BankerSK').enable();
          this.disableContact = false;
          this._peopleService.asyncEveryBody({
            filter: {
              logic: 'and',
              filters: kendoFilter([
                { field: 'VendorSK', value: item, operator: 'eq', logic: 'and' }
              ])
            }
          })
          this.fillContact(item);
        }
      })

    //poppulate vendor list according to bankersk
    if (this.formGroup.get('BankerSK').value != null) {

      this._subsink.sink = this._peopleService.everyBody({
        filter: {
          logic: 'and',
          filters: kendoFilter([
            {
              field: 'SysKey', value: this.formGroup.get('BankerSK').value, logic: 'and', operator: 'eq'
            }
          ])
        }
      }).subscribe((data: any[]) => {

        if (data.length > 0) {
          if (data[0].VendorSK) {

            this.formGroup.get('VendorSK').enable({ emitEvent: false });
            this.formGroup.get('VendorSK').patchValue(data[0].VendorSK, { emitEvent: false });

            this._peopleService.asyncEveryBody({
              filter: {
                logic: 'and',
                filters: kendoFilter([
                  {
                    field: 'VendorSK', value: data[0].VendorSK, operator: 'eq', logic: 'and'
                  }
                ])
              }
            });
            this.fillContact(data[0].VendorSK)
          }
        } else {
          this.formGroup.get('BankerSK').disable({ emitEvent: false });
        }
      });
    }
  }

  public itemDisabled(itemArgs: { dataItem: any, index: number }) {

    if (typeof itemArgs.dataItem.IsActive != 'undefined')
      return !itemArgs.dataItem.IsActive;

    return false;
  }

  private fillContact(key: string) {

    this._subsink.sink = this._contactService.findByKey(key)
      .subscribe((data: any) => {

        this.formGroup.get('BuildingHouseNo').setValue(data.BuildingHouseNo);
        this.formGroup.get('Address1').setValue(data.Address1);
        this.formGroup.get('Address2').setValue(data.Address2);
        this.formGroup.get('FloorUnit').setValue(data.FloorUnit);
        this.formGroup.get('PostalCode').setValue(data.PostalCode);
        this.formGroup.get('Country').setValue(data.Country);
      });
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  private _companyFilterText: string;
  handelCompanyFilterChange(param: { value: string }) {

    this._companyFilterText = param.value;

    if (param.value.length >= 3) {

      this._vendorService.asyncFinds({
        filter: {
          filters: kendoFilter(this.companyFilterSetting(param.value)),
          logic: 'and'
        }
      });
    }
  }

  private _bankerName: string;
  handelPeopleFilterChange(param: { value: string }) {

    this._bankerName = param.value;
    if (param.value.length >= 3) {
      this._peopleService.asyncEveryBody({
        filter: {
          logic: 'and',
          filters: kendoFilter(this.personFilterSetting({ name: param.value, companykey: this.formGroup.get('VendorSK').value }))
        }
      });
    }
  }

  companyFilterSetting(name: string = null) {

    let filterInfo = [];

    if (name != null) {
      filterInfo.push({ field: 'CompanyName', value: name, operator: 'contains', logic: 'and' });
    }

    filterInfo.push({ field: 'TransType', value: this.formGroup.get('ClientTransType').value, operator: 'eq', logic: 'and' });

    return filterInfo;
  }

  personFilterSetting(param: { key?: string, name?: string, companykey?: string }) {

    let filterInfo = [];

    if (param.companykey && param.companykey != null)
      filterInfo.push({ field: 'VendorSK', value: param.companykey, operator: 'eq', logic: 'and' });


    if (param.name && param.name != null)
      filterInfo.push({ field: 'Name', value: param.name, operator: 'contains', logic: 'and' });


    if (param.key && param.key != null)
      filterInfo.push({ field: 'SysKey', value: param.key, operator: 'eq', logic: 'and' });

    return filterInfo;
  }

  addNewCompany() {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'Vendor Management', componentType: VendorManagementDetailComponent, state: state });
    let windowComponent = window.content.instance as VendorManagementDetailComponent;
    windowComponent.loadData({ CompanyName: this._companyFilterText, TransType: this.formGroup.get('ClientTransType').value });

    this._subsink.sink = windowComponent.OnSuccess.
      subscribe(item => {

        this._vendorService.asyncFinds({
          filter: {
            filters: kendoFilter([
              { field: 'SysKey', value: item.data.Vendor.SysKey, logic: 'and', operator: 'eq' }
            ]),
            logic: 'and'
          }
        });
        this.formGroup.get('VendorSK').setValue(item.data.Vendor.SysKey);
        // this.selectedCompany = data.Vendor;
        this._windowService.close();
      });

  }

  addNewBanker() {

    let state: WindowState = 'maximized';
    let window = this._windowService.open({ title: 'External Salesperson Management', componentType: ExternalSalespersonDetailComponent, state: state });
    let windowComponent = window.content.instance as ExternalSalespersonDetailComponent;

    windowComponent.loadData({ VendorSK: this.formGroup.get('VendorSK').value, Name: this._bankerName });

    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(item => {

        if (item.data.Person.SysKey != null) {
          this._peopleService.asyncEveryBody({
            filter: {
              logic: 'and',
              filters: kendoFilter(this.personFilterSetting({ key: item.data.Person.SysKey }))
            }
          });
        }

        this.formGroup.get('BankerSK').setValue(item.data.Person.SysKey);
        this._windowService.close();
      });
  }

  filterSettings: DropDownFilterSettings = {
    caseSensitive: false,
    operator: 'contains'
  };
}
