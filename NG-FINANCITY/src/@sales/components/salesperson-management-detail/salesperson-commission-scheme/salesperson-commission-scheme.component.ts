import { Component, OnInit, Input, ViewEncapsulation, ViewChild, OnDestroy } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { CommissionStructureService } from 'src/@sales/services/commission-structure.service';
import { CommissionStructureDetailService } from 'src/@sales/services/commission-structure-detail.service';
import { DropDownFilterSettings } from '@progress/kendo-angular-dropdowns';
import { CommissionGridService } from './commission-grid.service';
import { TooltipDirective } from '@progress/kendo-angular-tooltip';
import { WindowState } from '@progress/kendo-angular-dialog';
import { CommissionSchemeDetailComponent } from '../../commission-scheme-detail/commission-scheme-detail.component';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';
import { kendoFilter, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-salesperson-commission-scheme',
  templateUrl: './salesperson-commission-scheme.component.html',
  encapsulation: ViewEncapsulation.None,
  styles: [`
      tr .text-center {
        text-align:center
      },
      tr .text-right {
          text-align:right
      }
      .k-grid .k-grid-content td {
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
      }
   `],
  providers: [CommissionGridService,
    CommissionStructureService,
    SalespersonManagementService,
    PWindowComponentService,
    CommissionStructureDetailService]
})
export class SalespersonCommissionSchemeComponent implements OnInit, OnDestroy {

  @ViewChild(TooltipDirective, { static: true }) public tooltipDir: TooltipDirective;
  @Input() formGroup: FormGroup;

  private _subsink = new SubSink();

  constructor(public fb: FormBuilder,
    private _salespersonService: SalespersonManagementService,
    public _windowService: PWindowComponentService,
    public _commissionScheme: CommissionStructureService,
    public _gridService: CommissionGridService,
    public _commissionSchemeDetail: CommissionStructureDetailService) {
  }

  ngOnInit() {

    let firstTime = true;
    //insert model control into sales group
    this.formGroup.addControl('SalespersonCommScheme', new FormControl());

    this._subsink.sink = this._commissionSchemeDetail.findByKey(this.formGroup.get('SchemeDetailSysKey').value)
      .subscribe(d => {
        this.formGroup.get('SalespersonCommScheme').patchValue(d.ParentSK);
      });

    this._subsink.sink = this.formGroup.get('SalespersonCommScheme').valueChanges
      .subscribe(com => {

        this._gridService.read(com, this.formGroup, this._commissionSchemeDetail).then(() => {

          if (firstTime && this.formGroup.get('SchemeDetailSysKey').value != null)
            this._gridService.setDesignation({ key: this.formGroup.get('SchemeDetailSysKey').value })
          else
            this._gridService.setDesignation();

          this.isTier(com).then((tier) => {

            if (tier == true)
              this.setCheckMarkState({ tier: tier })
            else {
              // this._gridService.formArray.controls.map((o, i) => {
              //   //clear all check marks
              //   (o['value']['IsCurrent'] as FormControl).setValue(false);
              // });
            }
          })
          firstTime = false;
        })
      })

    this.formGroup.addControl('CloseSalesCommissionToDate', new FormControl(0));
    this.formGroup.addControl('CloseSalesValueToDate', new FormControl(0));

    this.formGroup.addControl('CloseSalesCommissionToYear', new FormControl(0));
    this.formGroup.addControl('CloseSalesValueToYear', new FormControl(0));

    this.formGroup.addControl('GrossToPromoteNext', new FormControl(0));
    this.formGroup.addControl('ResetEffectiveDate', new FormControl(this.formGroup.get('ResetCommissionEffectiveStartDate').value != null));

    if (this.formGroup.get('ResetEffectiveDate').value)
      this.formGroup.get('ResetCommissionEffectiveStartDate').enable();
    else
      this.formGroup.get('ResetCommissionEffectiveStartDate').disable();

    this._commissionScheme.asyncFinds();

    this._subsink.sink = this._salespersonService.getCloseSalesCommissionToDate(this.formGroup.get('SysKey').value)
      .subscribe(d => {
        this.formGroup.get('CloseSalesCommissionToDate').setValue(d); //set close sales to date from api
      });

    this._subsink.sink = this._salespersonService.getCloseSalesCommissionToYear(this.formGroup.get('SysKey').value)
      .subscribe(d => {
        this.formGroup.get('CloseSalesCommissionToYear').setValue(d); //set close sales to year from api
      });

    this._subsink.sink = this._salespersonService.getCloseSalesValueToDate(this.formGroup.get('SysKey').value)
      .subscribe(d => {
        this.formGroup.get('CloseSalesValueToDate').setValue(d); //set close sales to date from api
      });

    this._subsink.sink = this._salespersonService.getCloseSalesValueToYear(this.formGroup.get('SysKey').value)
      .subscribe(d => {
        this.formGroup.get('CloseSalesValueToYear').setValue(d); //set close sales to year from api
      });

    this._subsink.sink = this.formGroup.get('ResetEffectiveDate').valueChanges
      .subscribe(c => {
        if (c == true)
          this.formGroup.get('ResetCommissionEffectiveStartDate').enable();
        else {
          this.formGroup.get('ResetCommissionEffectiveStartDate').setValue(null);
          this.formGroup.get('ResetCommissionEffectiveStartDate').disable();
        }
      });

    this.formGroup.get('OpeningAccSales').setValidators([Validators.required]);
    this.formGroup.get('SalespersonCommScheme').setValidators([Validators.required]);
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  handleDesignation(e: { event: any, index: number, data: any }) {

    this._gridService.setDesignation({ key: e.data.value.SysKey.value, explicitSetOpeningSales: e.data.value.Type.value == 4 });
  }

  filterSettings: DropDownFilterSettings = {
    caseSensitive: false,
    operator: 'contains'
  };

  private _commissionSchemefilterText: string;
  handelCommissionSchemeFilterChange(value: string) {

    this._commissionSchemefilterText = value;
  }

  public itemDisabled(itemArgs: { dataItem: any, index: number }) {
    return !itemArgs.dataItem.IsActive; //disable the 3rd item
  }

  addNewScheme() {

    const state: WindowState = 'maximized';
    const window = this._windowService.open({ title: 'Commission Structure Setup', componentType: CommissionSchemeDetailComponent, state: state });
    const windowComponent = window.content.instance as CommissionSchemeDetailComponent;
    windowComponent.loadData({ SchemeName: this._commissionSchemefilterText });

    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(item => {

        this._commissionScheme.asyncFinds();
        let key = item.data.CommissionScheme.SysKey;
        this.formGroup.get('SalespersonCommScheme').patchValue(key);
        //should read it initially to poppulate the grid
        this._gridService.read(key, this.formGroup, this._commissionSchemeDetail).then(() => {
          this._gridService.setDesignation();
        });
        this._windowService.close();
      });
  }

  showTooltip(e: MouseEvent): void {

    const element = e.target as HTMLElement;
    if ((element.nodeName === 'TD' || element.nodeName === 'TH')
      && element.offsetWidth < element.scrollWidth) {
      this.tooltipDir.toggle(element);
    } else {
      this.tooltipDir.hide();
    }
  }

  onChangeAccSales() {

    this.isTier(this.formGroup.get('SchemeDetailSysKey').value)
      .then((tier) => {

        if (tier)
          this._gridService.setDesignation();
      })
  }

  private isTier(headersk: string): Promise<boolean> {

    return new Promise((resolve) => {
      this._subsink.sink = this._commissionSchemeDetail.finds({
        filter: {
          logic: 'and',
          filters: kendoFilter([
            { field: 'ParentSK', value: headersk, operator: 'eq', logic: 'and' }
          ])
        }
      }).subscribe(data => {
        resolve(data[0]['Type'] == 4);
      })
    })
  }

  private setCheckMarkState(param: { tier: boolean }) {

    if (param.tier && this.formGroup.get('RecordStatus').value > 0) {
      this._gridService.setDisabled();
    }
    else if (param.tier) {
      this._gridService.setEnabled();
    }
    else {
      this._gridService.setEnabled();
      this.formGroup.get('GrossToPromoteNext').patchValue(0);
    }
  }
}
