import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { SubSink } from 'src/@common/utils';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { UtilityService } from 'src/app/services/util.service';

@Component({
  selector: 'fc-financial-calculator',
  templateUrl: './financial-calculator.page.html',
  providers: [UtilityService,
    FormUtilityService]
})
export class FinancialCalculatorPage implements OnInit, OnDestroy {

  public form: FormGroup;

  private _subsink = new SubSink();

  constructor(
    private _utilityService: UtilityService,
    private _formUtilityService: FormUtilityService,
    private _formBuilder: FormBuilder) {
  }

  ngOnDestroy(): void {

    this._subsink.unsubscribe();
  }

  ngOnInit() {

    const model = {
      PropertyValue: 0.00,
      DepositCPF: 0.00,
      DepositeCash: 0.00,
      LoneAmount: 0.00,
      LoneTenure: 0,
      InterestRate: 0,
      PaymentAmount: 0.00
    };

    let c = this._formUtilityService.formGroupJson(model, null, fg => {
    }, [
      'PropertyValue',
      'DepositCPF',
      'DepositeCash',
      'LoneAmount',
      'LoneTenure',
      'InterestRate',
      'PaymentAmount'
    ]);

    this.form = this._formBuilder.group(c);
  }

  handleValueChange() {

    console.log(this.form.get('LoneAmount').value)

    this.form.get('LoneAmount').setValue(this.form.get('PropertyValue').value -
      this.form.get('DepositCPF').value -
      this.form.get('DepositeCash').value);

    if (this.form.get('InterestRate').value > 0 &&
      this.form.get('LoneAmount').value > 0
      && this.form.get('PropertyValue').value > 0) {

      this._subsink.sink = this._utilityService.getPmt({
        rate: this.form.get('InterestRate').value,
        months: this.form.get('LoneTenure').value,
        loan: this.form.get('LoneAmount').value
      })
        .subscribe(data => {

          this.form.get('PaymentAmount').patchValue(data);
        });
    }
  }
}
