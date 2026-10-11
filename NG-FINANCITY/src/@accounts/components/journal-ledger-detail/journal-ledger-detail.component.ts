import { Component, OnInit, ViewChild, OnDestroy, Inject } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { BehaviorSubject } from 'rxjs';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { JournalLedgerService } from 'src/@accounts/services/journal-ledger-service';
import { Router, ActivatedRoute } from '@angular/router';
import { makeFormTouched, createJsonFormGroup, kendoFilter, cloneData, SubSink } from 'src/@common/service/utility-service';
import { PGridIncellEditingComponent } from 'src/@common/components/p-grid-incell-editing/p-grid-incell-editing.component';
import { WindowState } from '@progress/kendo-angular-dialog';
import { PWindowComponentService } from 'src/@common/components/p-window/p-window.component.service';
import { AccountSetupDetailComponent } from '../account-setup-detail/account-setup-detail.component';
import { AccountSetupService } from 'src/@accounts/services/account-setup.service';
import { BaseApiService } from 'src/@common/Infrastructure/base.api.service';
import { CurrencyRateService } from 'src/@sales/services/currency-rate.service';
import { CurrencyRateDetailComponent } from 'src/@sales/components/currency-rate-detail/currency-rate-detail.component';
import { CurrencyService } from 'src/@sales/services/currency.service';
import { RecapDailogComponent } from 'src/@common/components/p-recap-dialog/p-recap-dialog.component';
import { JobService } from 'src/@accounts/services/job.service';
import { JobDetailComponent } from '../job-detail/job-detail.component';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-journal-ledger-detail',
  templateUrl: './journal-ledger-detail.component.html',
  providers: [
    JournalLedgerService,
    PWindowComponentService,
    AccountSetupService,
    CurrencyRateService,
    CurrencyService,
    JobService
  ]
})
export class JournalLedgerDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PGridIncellEditingComponent, { static: true }) private _dataGrid: PGridIncellEditingComponent;
  @ViewChild(PToolbarComponent, { static: true }) private _toolbar: PToolbarComponent;
  @ViewChild('recapDailog', { static: true }) private _recap: RecapDailogComponent;

  private _subsink = new SubSink();
  private _accounts = [];
  private _tax = [];
  private _job = [];
  private _shouldInsert: boolean;
  private _shouldRemove: boolean;

  totalDebit = new BehaviorSubject(0);
  totalCredit = new BehaviorSubject(0);

  public get readonly(): boolean {
    return this.form.get('RecordStatus').value >= 8;
  }

  public baseCurr = environment.general.currSK;

  constructor(fb: FormBuilder,
    public _router: Router,
    private _jobService: JobService,
    public _currencyService: CurrencyService,
    public _currencyRateService: CurrencyRateService,
    private _windowService: PWindowComponentService,
    public _activatedRoute: ActivatedRoute,
    private _accountSetupService: AccountSetupService,
    private _journalLedgerService: JournalLedgerService,
  ) {

    super(fb);

    this.updateSource([]);

    this._subsink.sink = this._journalLedgerService
      .subscribe(() => {

        this.updateTotal();
      })

    this._subsink.sink = this.OnSuccess
      .subscribe((item) => {

        if (item.data.RecordStatus == 0) {
          this.updateSource([])
        } else {
          this.findGL({
            moduleCode: item.data.ModuleCode,
            parentId: item.data.ParentId,
            yc: item.data.YC,
            postDate: item.data.PostDate,
          })
        }
        this.updateTotal();
      })
  }

  ngOnInit() {

    this._currencyRateService.asyncFinds();

    this._toolbar
      .controls
      .Success(this.OnSuccess)
      .Ready(this.formReady)
      .StateKey(['RecordStatus'])
      .NavigationButton({ action: () => this._router.navigateByUrl('/Accounting/JournalLedger') })
      .NewButton({
        action: () =>
          this._journalLedgerService.default()
      })
      .SaveButton({
        action: () =>
          this._journalLedgerService.save(this.bindFormValue()),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .UpdateButton({
        action: () =>
          this._journalLedgerService.update(this.bindFormValue()),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .PostButton({
        action: () =>
          this._journalLedgerService.post(this.bindFormValue()),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .ReverseButton({
        action: () =>
          this._journalLedgerService.reverseGL({ parentId: this.form.value.ParentId, postDate: this.form.value.PostDate }),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .VoidButton({
        action: () =>
          this._journalLedgerService.voidGL({ parentId: this.form.value.ParentId, postDate: this.form.value.PostDate }),
        validate: () => { makeFormTouched(this.form); return this.form.valid }
      })
      .RecapButton({
        action: () =>
          this._recap.open(this._journalLedgerService.recap({
            parentId: this.form.value.ParentId,
            moduleCode: this.form.value.ModuleCode,
            postDate: this.form.value.PostDate
          }))
      });

    this._subsink.sink = this._activatedRoute.data
      .subscribe((data) => {

        this.loadData(data['data']['summary']);

        this._accounts.push(...data['data']['accounts']);
        this._tax.push({ SysKey: null, TaxCode: 'N/A', TaxPercent: 0 });
        this._tax.push(...data['data']['tax']);
        this._job.push({ SysKey: null, Description: 'N/A' });
        this._job.push(...data['data']['job']);

        this._dataGrid.config = {
          key: 'SysKey',
          fetch: this._journalLedgerService,
          columns: [
            { dataType: 'hidden', field: 'SysKey', defaultValue: null },
            {
              dataType: 'hidden', field: 'ParentId', defaultValue: null
            },
            {
              dataType: 'hidden', field: 'HasTaxLine', defaultValue: false
            },
            {
              dataType: 'hidden', field: 'Editable', defaultValue: true
            },
            {
              dataType: 'dropdown', field: 'AccSK', title: 'Account', width: 300, validators: Validators.required, dropdown: {
                fetch: this._accountSetupService,
                textField: 'AccCode',
                valueField: 'SysKey',
                itemTemplateField: {
                  field1: 'AccCode',
                  field2: 'AccDescription'
                },
                itemEmpty: (i) => this.addNewAccount(i),
                customFilter: (e) => {

                  if (e.value.length > 2) {
                    if (e.dropDown.fetch instanceof BaseApiService) {
                      e.dropDown.fetch
                        .asyncFinds({
                          filter: {
                            filters: kendoFilter([
                              { field: 'AccCode', value: `<# ${e.value}`, operator: 'contains', logic: 'or' },
                              { field: 'AccDescription', value: `${e.value} #>`, operator: 'contains', logic: 'and' },
                              { field: 'AccountType', value: 1, operator: 'eq', logic: 'and' },
                              { field: 'IsDefault', value: false, operator: 'eq', logic: 'and' },
                            ]),
                            logic: 'or'
                          }
                        })
                    }
                  }
                }
              }
            },
            {
              dataType: 'currency', field: 'Debit', validators: Validators.required, title: 'Debit', width: 120, defaultValue: 0,
              cellValueChange: (c) => {

                //should prevent inserting or removing signature for updated event.
                this._shouldInsert = false;
                this._shouldRemove = false;

                c.fg.get('Credit').patchValue(0);
                if (c.fg.get('HasTaxLine').value) {

                  let percent = c.fg.get('TaxPercent').value;
                  let item = (this._journalLedgerService.value as any[])[c.rowIndex + 1];

                  item['Credit'] = 0;
                  item['Debit'] = (c.fg.get('Debit').value / 100) * percent;
                  this._dataGrid.service.update(item);
                }
              }
            },
            {
              dataType: 'currency', field: 'Credit', validators: Validators.required, title: 'Credit', width: 120, defaultValue: 0,
              cellValueChange: (c) => {
                c.fg.get('Debit').patchValue(0);
                if (c.fg.get('HasTaxLine').value) {

                  let percent = c.fg.get('TaxPercent').value;
                  let item = (this._journalLedgerService.value as any[])[c.rowIndex + 1];

                  item['Debit'] = 0;
                  item['Credit'] = (c.fg.get('Credit').value / 100) * percent;
                  this._dataGrid.service.update(item);
                }
              }
            },
            {
              dataType: 'dropdown', field: 'TaxSK', title: 'GST', width: 120, dropdown: {
                fetch: this._tax,
                textField: 'TaxCode',
                valueField: 'SysKey',
              }, cellValueChange: (c) => {

                let t = this._tax.find(f => f.SysKey == c.event);
                let old_taxline = c.fg.get('HasTaxLine').value;
                let new_percent = t['TaxPercent'];

                c.fg.get('TaxSK').patchValue(c.event);
                c.fg.get('TaxPercent').patchValue(new_percent);

                //trigger the tax line should be insert into.
                //after cell vale change event, the updateCompleted event will fire.
                //at this point we should perform insertion or removing tax information row.
                if (old_taxline == true && new_percent == 0) {
                  this._shouldRemove = true;
                }

                //trigger the tax line should be insert into.
                //after cell vale change event, the updateCompleted event will fire.
                //at this point we should perform insertion or removing tax information row.
                if (!old_taxline == true && new_percent > 0) {
                  this._shouldInsert = true;
                }

                if (new_percent > 0)
                  c.fg.get('HasTaxLine').patchValue(true);
                else
                  c.fg.get('HasTaxLine').patchValue(false);
              }
            },
            { dataType: 'display', field: 'TaxPercent', title: 'GST (%)', width: 120, defaultValue: 0 },
            {
              dataType: 'dropdown', field: 'JobSK', title: 'Job', width: 200, dropdown: {
                fetch: this._job,
                textField: 'Description',
                valueField: 'SysKey',
                itemEmpty: (i) => this.addJob(i)
              }
            },
            { dataType: 'string', field: 'LineDescription', title: 'Memo' },
          ],
          rowDataBound: (b) => {
            // do nothing with on row data bound.
            // as this GL grid is not appropriate to do inside rowdatabound.
            // that you can adjust with the GL's asyncFindGL() to add or remove additional data fields.
          },
          saveCompleted: (b) => {

            let fg = this.prepareTaxLine(b.formGroup);
            if (fg != null)
              this._dataGrid.service.create(fg.value);

            this.updateTotal();
          },
          updateCompleted: (b) => {

            let source = cloneData(this._journalLedgerService.value as any[]);

            //What ever data update will enter inside this scope. 
            //At this point you should do remove or add rows for GST information.
            //insert tax information row.

            if (this._shouldInsert == true) {
              this._shouldInsert = false;
              if (source[b.rowIndex]['HasTaxLine'] == true) {

                let fg = this.fb.group(
                  source[b.rowIndex])

                let n = this.prepareTaxLine(fg);
                // const k = n['ParentId'] == null ? 0 : 1;
                if (n != null) {
                  source.splice(b.rowIndex + 1, 0, n.value);
                }
              }
            }

            //remove tax information row.
            if (this._shouldRemove == true) {
              this._shouldRemove = false;
              source.splice(b.rowIndex + 1, 1);
            }

            this.updateSource(source);
            this.updateTotal();
          },
          deleteCompleted: (b) => {

            //Remove GST information if it is exist in the next row.
            if ((this._journalLedgerService.value as any[]).length != b.rowIndex) {

              let item = (this._journalLedgerService.value as any[])[b.rowIndex];
              if (typeof item['Editable'] != 'undefined' && !item['Editable']) {
                this._dataGrid.service.remove(item, b.rowIndex);
              }
            }

            this.updateTotal();
          },
          canEditRow: (b) => {

            //Determine the current row should be editable or not.
            return !(typeof b.dataItem['Editable'] != 'undefined'
              && b.dataItem['Editable'] == false
              && b.columnIndex != 1
              && b.columnIndex != 2
              && b.columnIndex != 5);
          },
          canDeleteRow: (b) => {

            //Determine the current row should be deletable or not.
            return (typeof b.dataItem['Editable'] != 'undefined' && b.dataItem['Editable'] == false);
          }
        }

        if (data['data']['summary'].ModuleCode != null) {
          this.findGL({

            moduleCode: data['data']['summary'].ModuleCode,
            parentId: data['data']['summary'].ParentId,
            yc: data['data']['summary'].YC,
            postDate: data['data']['summary'].PostDate,
          });
        }
      })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  private findGL({ moduleCode, parentId, yc, postDate }) {

    //subscribing to the journal ledger service which interlink with the datagrid.
    this._journalLedgerService
      .asyncFindGL({
        moduleCode: moduleCode,
        ycpc: yc,
        parentId: parentId,
        postDate: postDate
      })
  }

  //refresh total debit and credit amount.
  private updateTotal() {

    let total = cloneData(this._journalLedgerService.value as any[])

    this.totalDebit.next(total.reduce((sum, current) => sum + current['Debit'], 0));
    this.totalCredit.next(total.reduce((sum, current) => sum + current['Credit'], 0));
  }

  private addNewAccount(i?: (j: any) => any) {

    let state: WindowState = 'maximized';
    let window = this._windowService.open({ title: 'Account Setup', componentType: AccountSetupDetailComponent, state: state });
    let windowComponent = window.content.instance as AccountSetupDetailComponent;

    windowComponent.loadData({ AccDescription: this._dataGrid.dropDownFilterText });

    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(item => {
        if (i) {
          this._windowService.close();
          i(item.data.SysKey);
        }
        window.close();
      })
  }

  /**
  * Preparing the Tax information line.
  * @param fg form group
  */
  private prepareTaxLine(fg: FormGroup) {

    let t = this._tax.find(f => f.SysKey == fg.get('TaxSK').value);
    let percent = t['TaxPercent'];
    if (percent > 0) {

      let accsk = '';
      let credit = fg.get('Credit').value;
      let debit = fg.get('Debit').value;

      if (credit > 0) {

        credit = (credit / 100) * percent;
        fg.get('Credit').patchValue(credit);
        accsk = t['OutputTaxSK'];
      }
      else {

        debit = (debit / 100) * percent;
        fg.get('Debit').patchValue(debit);
        accsk = t['InputTaxSK'];
      }

      fg.get('HasTaxLine').patchValue(false);
      fg.get('AccSK').patchValue(accsk);
      fg.get('TaxSK').patchValue(null);
      fg.get('TaxPercent').patchValue(0);
      fg.get('LineDescription').patchValue(null);
      fg.get('Editable').patchValue(false);

      return fg;
    }
    return null;
  }

  private loadData(data: any): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve) => {

      const c: any = createJsonFormGroup(data);
      this.form = this.fb.group(c);

      this.form.get('PostDate').setValidators([Validators.required]);

      this._subsink.sink = this.form.get('CURRSK').valueChanges
        .subscribe(d => {
          const currate = (this._currencyRateService.value as any[]).find(x => x.SysKey == d);
          this.form.get('CURRATE').patchValue(currate['Rate']);
          this.form.get('CURROP').patchValue(currate['Operator'])
        })

      resolve(null);
    })
  }

  private updateSource(data: any[]) {

    this._journalLedgerService.next(data);
  }

  private bindFormValue() {

    let source = cloneData(this._journalLedgerService.value as []);

    source.map((m, i) => {

      m['R1'] = this.form.get('R1').value;
      m['SecondRef'] = this.form.get('SecondRef').value;
      m['ParentId'] = this.form.get('ParentId').value;
      m['PostDate'] = this.form.get('PostDate').value;
      m['EntryDescription'] = this.form.get('Description').value;
      m['BatchNo'] = this.form.get('BatchNo').value;
      m['ModuleCode'] = 'GLC';
      m['IsAutoOpening'] = this.form.get('IsOpening').value;
      m['RecordStatus'] = this.form.get('RecordStatus').value;
      m['CURRSK'] = this.form.get('CURRSK').value;
      m['CURRATE'] = this.form.get('CURRATE').value;
      m['CURROP'] = this.form.get('CURROP').value;

      if (m['Debit'] > 0)
        m['TransactionAmount'] = m['Debit'];

      if (m['Credit'] > 0)
        m['TransactionAmount'] = m['Credit'] * (-1);

      if (m['HasTaxLine'] == true && m['TaxPercent'] > 0) {

        let taxLine = source[i + 1];

        taxLine['TaxCode'] = m['TaxCode'];
        taxLine['TaxPercent'] = m['TaxPercent'];
        taxLine['TaxSK'] = m['TaxSK'];
        taxLine['TaxAmount'] = m['Debit'] - (m['Credit'] * (-1));

        m['TaxCode'] = 'N/A';
        m['TaxPercent'] = 0;
        m['TaxSK'] = null;
        m['TaxAmount'] = 0;
      }
    })
    return source;
  }

  private _currencyFilterText: string;
  addNewCurrency() {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'Currency Setup', componentType: CurrencyRateDetailComponent, state: state });
    let windowComponent = window.content.instance as CurrencyRateDetailComponent;
    windowComponent.loadData({ CurrencyName: this._currencyFilterText });

    this._subsink.sink = windowComponent.OnSuccess
      .subscribe((data: any) => {

        this._currencyRateService.asyncFinds();

        this.form.get('CURRSK').setValue(data.SysKey);
        this.form.get('CURRATE').setValue(data.Rate);
        this.form.get('CURROP').setValue(data.Operator);

        this._windowService.close();
      });
  }

  handelCurrencyCodeFilterChange(value: string) {

    this._currencyService.asyncFinds({
      filter: {
        logic: 'or',
        filters: this.currencyDropDownSetting(value)
      }
    });
  }

  addJob(i?: (j: any) => any) {

    let state: WindowState = 'default';
    let window = this._windowService.open({ title: 'Job', componentType: JobDetailComponent, state: state });
    let windowComponent = window.content.instance as JobDetailComponent;

    windowComponent.loadData({ Description: this._dataGrid.dropDownFilterText });
    this._subsink.sink = windowComponent.OnSuccess
      .subscribe(item => {

        this._job.push(item);
        if (i) {
          i(item.data.SysKey);
        }
        this._windowService.close();
      });
  }

  currencyDropDownSetting(value: string) {

    return kendoFilter([
      { field: 'Currency_Name', value: value, operator: 'contains', logic: 'or' },
      { field: 'Currency_AlphaCode', value: value, operator: 'contains', logic: 'or' },
      { field: 'Country', value: value, operator: 'contains', logic: 'or' }
    ])
  }
}
