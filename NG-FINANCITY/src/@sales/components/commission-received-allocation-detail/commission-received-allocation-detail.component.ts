import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder } from '@angular/forms';
import { PModalFormComponent } from 'src/@common/components/p-modal-form/p-modal-form.component';
import { from } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { CommissionReceivedService } from 'src/@sales/services/commission-received.service';
import { createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-commission-received-allocation-detail',
  templateUrl: './commission-received-allocation-detail.component.html',
  providers: [CommissionReceivedService]
})
export class CommissionReceivedAllocationDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  public _subsink = new SubSink();

  @ViewChild(PModalFormComponent, { static: true }) public _modalForm: PModalFormComponent;
  @ViewChild(PToolbarComponent, { static: true }) _toolbar: PToolbarComponent;

  constructor(fb: FormBuilder,
    public _router: Router,
    public _activatedRoute: ActivatedRoute,
    public _commissionReceivedService: CommissionReceivedService,
  ) {

    super(fb);

    this._commissionReceivedService.next([]);
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  ngOnInit() {

    this._toolbar
      .controls
      .NavigationButton({ action: () => from(this._router.navigateByUrl('Business/CommissionReceivedAllocation')) })

    this._subsink.sink = this._activatedRoute.data
      .subscribe((data) => {
        this.loadData(data['data']);
      });
  }

  loadData(data: any) {

    const c: any = createJsonFormGroup(data);
    this.form = this.fb.group(c);
  }
}
