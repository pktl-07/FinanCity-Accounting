import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { ModalController } from '@ionic/angular';
import { SalespersonListModalPage } from '../salesperson-list-modal/salesperson-list-modal.page';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { getComissionContext, SubSink } from 'src/@common/utils';
import { SalespersonManagementService } from 'src/app/services/salesperson-management.service';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { CommissionStructureDetailService } from 'src/app/services/commission-structure-detail.service';

@Component({
  selector: 'fc-salesperson-sales-modal',
  templateUrl: 'salesperson-sales-modal.page.html',
  providers: [FormUtilityService,
    SalespersonManagementService,
    CommissionStructureDetailService,
  ]
})
export class SalespersonSalesModalPage implements OnInit, OnDestroy {

  @Input() form: FormGroup;
  @Input() transactionAmout: number = 0;
  @Input() disableSalespersonOption = false;

  private _subsink = new SubSink();
  private _timesout = [];

  commissionType = [];
  commissionTypeSubject = new BehaviorSubject([]);
  refreshed: boolean = false;

  constructor(private _modalController: ModalController,
    private _salespersonManagementService: SalespersonManagementService,
    public _commissionStructureDetailService: CommissionStructureDetailService,
    private _formUtilityService: FormUtilityService) {
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
    this._timesout.forEach((timeout) => clearTimeout(timeout))
  }

  ngOnInit() {

    this._subsink.sink = this._commissionStructureDetailService.finds()
      .pipe(map((response: any[]) => {
        response.map(m => {
          if (m['Type'] == 4)
            m['Designation'] = 'TIER';
        })
        return response;
      })).subscribe((data: any[]) => {
        this.commissionType = data;
        this.setDefaultCommission(this.form.get('SchemeDetailSK').value);
      })

    this._subsink.sink = this.form.get('SPSK').valueChanges
      .subscribe((spsk) => {
        this._subsink.sink = this._salespersonManagementService.getCurrentCommSchemeByKey(spsk)
          .subscribe((scheme) => {
            this.form.get('SchemeDetailSK').patchValue(scheme['SysKey']);
            this.setDefaultCommission(scheme['SysKey']);
          })
      })

    this.form.get('SalespersonName').setValidators([Validators.required]);
    this.form.get('SharePercent').setValidators([Validators.required, Validators.min(0), Validators.max(100)]);
    this.form.get('GrossComm').setValidators([Validators.required, Validators.min(0)]);
    this.form.get('TotalPayable').setValidators([Validators.required, Validators.min(0)]);
    this.form.get('SchemeDetailSK').setValidators([Validators.required]);
  }

  setDefaultCommission(schemeid: string) {

    this.refreshed = false;

    if (schemeid != null) {

      let scheme = this.commissionType.find(x => x.SysKey == schemeid);
      let parentsk = this.commissionType.find(x => x.SysKey == schemeid)['ParentSK'];

      if (scheme['Type'] == 4)
        this.commissionTypeSubject.next([scheme]);
      else
        this.commissionTypeSubject.next(this.commissionType.filter(x => x.ParentSK == parentsk));

    } else
      this.commissionTypeSubject.next([]);

    this._timesout.push(setTimeout(() => {
      this.refreshed = true;
    }))
  }

  async handleSelect() {

    const m = await this._modalController.create({
      component: SalespersonListModalPage,
    })

    m.onWillDismiss().then((item: any) => {

      if (item.data) {

        this.form.get('SalespersonName').patchValue(item.data.Name);
        this.form.get('SPSK').patchValue(item.data.SysKey);
      }
      this.form.get('SalespersonName').markAsTouched();
    })

    return await m.present();
  }

  handleSharePercent() {

    let rate = this.form.get('SharePercent').value;
    let price = this.transactionAmout;
    let tax = this.form.get('TaxPercent').value;
    let gross = (price / 100) * rate;
    let context = getComissionContext({ tax: tax, amount: gross, call: 'gross' })

    this.form.get('GrossComm').patchValue(gross);
    this.form.get('GSTValue').patchValue(context.taxAmount);
    this.form.get('TotalPayable').patchValue(context.total);
  }

  handleGrossComm() {

    let price = this.transactionAmout;
    let gross = this.form.get('GrossComm').value;
    let tax = this.form.get('TaxPercent').value;
    let context = getComissionContext({ tax: tax, amount: gross, call: 'gross' })

    this.form.get('SharePercent').patchValue((gross / price) * 100);
    this.form.get('GSTValue').patchValue(context.taxAmount);
    this.form.get('TotalPayable').patchValue(context.total);
  }

  handleTotalAmount() {

    let total = this.form.get('TotalPayable').value;
    let price = this.transactionAmout;
    let tax = this.form.get('TaxPercent').value;
    let context = getComissionContext({ tax: tax, amount: total, call: 'total' })

    this.form.get('SharePercent').patchValue((context.gross / price) * 100);
    this.form.get('GSTValue').patchValue(context.taxAmount);
    this.form.get('GrossComm').patchValue(context.gross);
  }

  handleClose() {
    this._modalController.dismiss();
  }

  handleOK() {

    this._formUtilityService.makeFormTouched(this.form);

    if (this.form.valid)
      this._modalController.dismiss(this.form.value);
  }

  compareWith = (o1, o2) => {
    return o1 && o2 ? o1.SysKey === o2.SysKey : o1 === o2;
  };
}
