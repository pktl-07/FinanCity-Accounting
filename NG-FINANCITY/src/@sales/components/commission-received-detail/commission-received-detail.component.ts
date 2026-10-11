import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { DetailFormBase } from 'src/@common/Infrastructure/detail.form.base';
import { FormBuilder } from '@angular/forms';
import { from } from 'rxjs';
import { Router, ActivatedRoute } from '@angular/router';
import { PToolbarComponent } from 'src/@common/components/p-toolbar/p-toolbar.component';
import { createJsonFormGroup, SubSink } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-commission-received-detail.component',
  templateUrl: './commission-received-detail.component.html'
})
export class CommissionReceivedDetailComponent extends DetailFormBase implements OnInit, OnDestroy {

  @ViewChild(PToolbarComponent, { static: true })
  _toolbar: PToolbarComponent;

  private _subsink = new SubSink();

  constructor(fb: FormBuilder,
    public _router: Router,
    public _activatedRoute: ActivatedRoute) {

    super(fb);
  }

  ngOnDestroy(): void {
    this._subsink.unsubscribe();
  }

  ngOnInit() {

    this._toolbar.controls
      .NavigationButton({ action: () => from(this._router.navigateByUrl('Business/CommissionReceived')) })

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
