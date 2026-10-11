import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { Router, ActivatedRoute } from '@angular/router';
import { CommissionPayableService } from 'src/@sales/services/commission-payable.service';
import { createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';
import { from } from 'rxjs';
import { PropertyTransactionService } from 'src/@sales/services/property-transaction.service';

@Component({
  selector: 'app-commission-payable-detail.component',
  templateUrl: './commission-payable-detail.component.html',
  providers: [CommissionPayableService]
})
export class CommissionPayableDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;

  public _subsink = new SubSink();

  constructor(fb: FormBuilder,
    public _propertyTransaction: PropertyTransactionService,
    public _activatedRoute: ActivatedRoute,
    public _router: Router,
    public _commissionPayableService: CommissionPayableService) {

    super(fb);
  }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => from(this._router.navigateByUrl('Business/CommissionPayable')) });

    this._subsink.sink = this._activatedRoute.data
      .subscribe((data) => {
        this.loadData(data['data']);
      });
  }

  loadData(data: any): Promise<FormGroup> {

    return new Promise<FormGroup>((resolve, reject) => {
      try {

        const c: any = createJsonFormGroup(data);
        this.form = this.fb.group(c);
        resolve(this.form)
      } catch (error) {
        reject(error);
      }
    })
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }
}
