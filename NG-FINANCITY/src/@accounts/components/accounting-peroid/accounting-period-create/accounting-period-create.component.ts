import { Component, OnInit, OnDestroy } from '@angular/core';
import { AccountPeriodService } from 'src/@accounts/services/account-period.service';
import { mergeMap, map } from 'rxjs/operators';
import { SubSink, zeroPrepending } from 'src/@common/service/utility-service';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-accounting-period-create',
  templateUrl: './accounting-period-create.component.html',
  providers: [
    AccountPeriodService
  ]
})
export class AccountingPeriodCreateComponent implements OnDestroy, OnInit {

  private _subsink = new SubSink();
  public form: FormGroup;

  constructor(private _fb: FormBuilder,
    public _accountingPeriodService: AccountPeriodService) {
  }

  yearcode: string;
  fromdate: any;
  todate: any;

  ngOnInit(): void {

    this.form = this._fb.group({
      'YearCode': new FormControl(),
      'FromDate': new FormControl(),
      'ToDate': new FormControl(),
      'PeriodType': new FormControl()
    });

    this._subsink.sink = this._accountingPeriodService.getLastAccountingYearCode()
      .pipe(mergeMap((code: string) => {
        return this._accountingPeriodService.getLastAccountingYear()
          .pipe(map((response: number) => {
            response = response == null ? new Date().getUTCFullYear() - 1 : response;
            return {
              yearcode: `${zeroPrepending(parseInt(code) + 1, 2)} [${response + 1}]`,
              fromdate: parseInt(`${response + 1}0101`),
              todate: parseInt(`${response + 1}1231`)
            }
          }))
      })
      ).subscribe(data => {

        this.form.get('YearCode').patchValue(data.yearcode);
        this.form.get('FromDate').patchValue(data.fromdate);
        this.form.get('ToDate').patchValue(data.todate);
        this.form.get('PeriodType').patchValue('Monthly');
      })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }
}
