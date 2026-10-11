import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { FormUtilityService } from 'src/app/services/form-utility.service';
import { SubSink } from 'src/@common/utils';

@Component({
  selector: 'fc-my-commission-detail',
  templateUrl: './my-commission-detail.page.html',
  providers: [
    FormUtilityService,
  ]
})
export class MyCommissiontDetailPage implements OnInit, OnDestroy {

  public form: FormGroup;
  private _subsink = new SubSink();

  constructor(private _activeRoute: ActivatedRoute,
    private _formUtilityService: FormUtilityService,
    private _formBuilder: FormBuilder) {
  }

  ngOnInit() {
    this._subsink.sink = this._activeRoute.data
      .subscribe((data) => {
        if (data && data['commission']) {
          let c = this._formUtilityService.formGroupJson(data['commission'],
            null,
            fg => { },
            [
              'LoanAmount',
              'GrossComm',
              'MarketingExpense',
              'AgencyAmount',
              'PayableAmount',
              'AmountPaid',
              'Balance'
            ]);
          this.form = this._formBuilder.group(c);
        }
      })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }
}
