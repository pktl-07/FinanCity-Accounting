import { Component, OnInit, OnDestroy, Inject } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { BroadcasterService } from 'src/app/services/broadcaster.service';
import { ActivatedRoute } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { PageUtilityService } from './page-utility.service';
import { PropertyTransactionService } from 'src/app/services/property-transaction.service';
import { ActionSheetController } from '@ionic/angular';
import { getDeletedItems, IFCConfig, getComissionContext, SubSink } from 'src/@common/utils';
import { cloneData } from 'src/@common/common.constant';
import { CommissionStructureService } from 'src/app/services/commission-structure.service';
import { SalespersonManagementService } from 'src/app/services/salesperson-management.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'fc-property-transaction-detail',
  templateUrl: './property-transaction-detail.page.html',
  providers: [FormUtilityService,
    PropertyTransactionService,
    CommissionStructureService,
    SalespersonManagementService]
})
export class PropertyTransactionDetailPage implements OnInit, OnDestroy {

  form: FormGroup;

  private _subsink = new SubSink();
  private _clientOriginValue: any[] = [];
  private _salespersonOriginValue: any[] = [];
  private _externalSalespersonOriginValue: any = [];
  private _marketingPromotionItem: any = [];

  constructor(private _activeRoute: ActivatedRoute,
    private _formBuilder: FormBuilder,
    private _salespersonManagementService: SalespersonManagementService,
    private _formUtilityService: FormUtilityService,
    private _propertyTransactionService: PropertyTransactionService,
    private _broadcaster: BroadcasterService,
    private _pageUtilityService: PageUtilityService,
    private _actionSheetController: ActionSheetController) { }

  ngOnInit() {

    this._subsink.sink = this._formUtilityService.OnSuccess
      .subscribe((data: any) => {

        if (data['PropertyTransaction']['RecordStatus'] != 0)
          this._broadcaster.broadcast('property-transaction-reload');

        this._formUtilityService.rebind(this.form, data);
        this.initNotifyObject(data);
      })

    this._subsink.sink = this._activeRoute.data
      .subscribe((data) => {
        this.bindControl(data['propertyTransaction']);
      })

    //whenever tax change it show reflect to internal salesperson total payable amount.
    this._subsink.sink = this._pageUtilityService.taxSubject
      .subscribe((tax) => {
        this._pageUtilityService.internalSalespersonSubject.value.forEach((internal) => {
          internal['TaxSK'] = tax.key;
          internal['TaxPercent'] = tax.percent;
          let gross = internal['GrossComm'];
          let context = getComissionContext({ tax: tax.percent, amount: gross, call: 'gross' })
          internal['GSTValue'] = context.taxAmount;
          internal['TotalPayable'] = context.total;
        })
      })

    this._subsink.sink = this.form.get('TNC').get('LoanAmount').valueChanges
      .subscribe(value => {
        this.form.get('PropertyTransaction').get('TransactedPrice').patchValue(value);
      })

    this._subsink.sink = this.form.get('PropertyTransaction').get('TransactedPrice').valueChanges
      .subscribe(value => {

        let rate = this.form.get('PropertyTransaction').get('CommissionRate').value;
        let tax = this.form.get('PropertyTransaction').get('TaxPercent').value;
        let gross = (value / 100) * rate;
        let context = getComissionContext({ tax: tax, amount: gross, call: 'gross' });

        this.form.get('PropertyTransaction').get('GrossCommission').patchValue(gross);
        this.form.get('PropertyTransaction').get('TaxAmount').patchValue(context.taxAmount);
        this.form.get('PropertyTransaction').get('TotalAmount').patchValue(context.total);
      })
  }
  /**
   * To destory subscribe objects.
   */
  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  handleTransactionPriceChange(value: any) {
    this.form.get('TNC').get('LoanAmount').setValue(value);
  }
  /**
   * To bind data with controls.
   * @param data routing data come from resolver service.
   */
  bindControl(data: any) {

    let c = this._formUtilityService.formGroupJson(data, null, fg => {
    }, [
      'LoanAmount',
      'RateTypeMargin',
      'LoanTenure',
      'TransactedPrice',
      'GrossCommission',
      'TaxAmount',
      'TotalAmount'
    ])

    this.form = this._formBuilder.group(c);

    this.initNewDefault(this.form);
    this.initNotifyObject(data);
  }

  /**
   * Initialize notify object for the property transaction page service.
   * @param data Current form group data.
   */
  private initNotifyObject(data: any) {

    this._clientOriginValue = cloneData(data['Clients']);
    this._salespersonOriginValue = cloneData(data['Internals']);
    this._externalSalespersonOriginValue = cloneData(data['Externals']);
    this._marketingPromotionItem = cloneData(data['PromotionExpense']);

    this._pageUtilityService.grossCommissionAmountSubject.next(data['PropertyTransaction']['GrossCommission']);
    this._pageUtilityService.PTOSKSubject.next(data['PropertyTransaction']['SysKey']);
    this._pageUtilityService.clientSubject.next(data['Clients']);
    this._pageUtilityService.externalSalespersonSubject.next(data['Externals']);
    this._pageUtilityService.internalSalespersonSubject.next(data['Internals']);
    this._pageUtilityService.marketingItemSubject.next(data['PromotionExpense']);
  }

  /**
   * To get default salesperson.
   * @param form current form group.
   */
  private getDefaultSalesperson(form: FormGroup): Promise<any> {

    return new Promise((resolve) => {

      this._subsink.sink = this._salespersonManagementService.getCurrentCommSchemeByKey(environment.auth.SPSK)
        .subscribe(data => {

          resolve({
            SysKey: null,
            SPSK: environment.auth.SPSK,
            IsMain: true,
            PTOSK: form.get('PropertyTransaction').get('SysKey').value,
            GrossComm: 0,
            GSTValue: 0,
            TotalPayable: 0,
            TaxSK: form.get('PropertyTransaction').get('TaxSK').value,
            TransType: 30,
            TaxPercent: form.get('PropertyTransaction').get('TaxPercent').value,
            VendorSK: '1',
            SchemeDetailSK: data['SysKey'],
          })
        })
    })
  }

  /**
   * Define login user as main salesperson default.
   * @param form current form group.
   */
  private initNewDefault(form: FormGroup) {
    let status = form.get('PropertyTransaction').get('RecordStatus').value == 0 ? -10
      : form.get('PropertyTransaction').get('RecordStatus').value;
    form.get('PropertyTransaction').get('RecordStatus').patchValue(status); //set record statu to -10 to show submit draft for action sheet buttons.
    if (status == -10) {
      this.getDefaultSalesperson(form).then((data) => {
        this._pageUtilityService.internalSalespersonSubject.next([
          data
        ])
      })
    }
    else {
      this._pageUtilityService.internalSalespersonSubject.next(this.form.get('Internals').value);
    }
  }

  /**
   * To integration of grid data for submision.
   */
  public bindFormValue(): any {

    return {

      Property: this.form.get('PropertyTransaction').value,
      Contact: this.form.get('Contact').value,
      TNC: this.form.get('TNC').value,


      ClientInsertedItems: this._pageUtilityService.clientSubject.value.filter(x => x['SysKey'] == null),
      ClientUpdatedItems: this._pageUtilityService.clientSubject.value.filter(x => x['SysKey'] != null),
      ClientDeletedItems: getDeletedItems(this._clientOriginValue,
        this._pageUtilityService.clientSubject.value, 'SysKey'),

      InternalInsertedItems: this._pageUtilityService.internalSalespersonSubject.value.filter(x => x['SysKey'] == null),
      InternalUpdatedItems: this._pageUtilityService.internalSalespersonSubject.value.filter(x => x['SysKey'] != null),
      InternalDeletedItems: getDeletedItems(this._salespersonOriginValue,
        this._pageUtilityService.internalSalespersonSubject.value, 'SysKey'),

      ExternalInsertedItems: this._pageUtilityService.externalSalespersonSubject.value.filter(x => x['SysKey'] == null),
      ExternalUpdatedItems: this._pageUtilityService.externalSalespersonSubject.value.filter(x => x['SysKey'] != null),
      ExternalDeletedItems: getDeletedItems(this._externalSalespersonOriginValue,
        this._pageUtilityService.externalSalespersonSubject.value, 'SysKey'),

      PromotionItemExpenseInsertedItems: this._pageUtilityService.marketingItemSubject.value.filter(x => x['SysKey'] == null),
      PromotionItemExpenseUpdatedItems: this._pageUtilityService.marketingItemSubject.value.filter(x => x['SysKey'] != null),
      PromotionItemExpenseDeletedITems: getDeletedItems(this._marketingPromotionItem,
        this._pageUtilityService.marketingItemSubject.value, 'SysKey'),
    }
  }

  /**
   * Action sheet button actions.
   */
  action() {
    const config = this._formUtilityService.build({
      form: this.form,
      status: +this.form.get('PropertyTransaction').get('RecordStatus').value,
      newHandler: this._propertyTransactionService.default().pipe(
        finalize(() => {
          this.initNewDefault(this.form);
        })),
      saveDraftHandler: this._propertyTransactionService.saveDraft(this.bindFormValue()),
      submitDraftHandler: this._propertyTransactionService.submit(this.bindFormValue()),
      resubmitHandler: this._propertyTransactionService.submit(this.bindFormValue()),
      updateHandler: this._propertyTransactionService.update(this.bindFormValue()),
      deleteHandler: this._propertyTransactionService.delete(this.form.get('PropertyTransaction').get('SysKey').value)
    });

    this._subsink.sink = config.subscribe(async (options) => {
      if (options.valid) {
        const actionSheet = await this._actionSheetController.create(options.config);
        await actionSheet.present();
      }
    })
  }
}
