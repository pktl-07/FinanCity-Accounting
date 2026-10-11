import { Component, OnInit, Input } from '@angular/core';
import { SalutationSource, GenderSource, MartialStatusSource, InformationSource, SaleActivity } from 'src/@common/common.constant';
import { NameConventionService } from 'src/app/services/name-conventions.service';
import { FormGroup, Validators } from '@angular/forms';
import { TNCPropertyTypeService } from 'src/app/services/tnc-property-type.service';
import { LeadCloseRate, notifyMeBefore } from 'src/@common/utils';

@Component({
  selector: 'fc-lead',
  templateUrl: './lead.page.html',
  providers: [NameConventionService,
    TNCPropertyTypeService
  ]
})
export class LeadPage implements OnInit {

  @Input() person: FormGroup;
  @Input() lead: FormGroup;
  @Input() followup: FormGroup;

  public salutation = [];
  public gender = [];
  public martialStatus = [];
  public salesActivities = [];
  public notifyMeBefore = []
  public source = [];
  public leadCloseRate = [];

  constructor(private _nameConventionService: NameConventionService,
    public _propertyTypeService: TNCPropertyTypeService,) {
  }

  ngOnInit() {

    this.salutation = SalutationSource;
    this.gender = GenderSource;
    this.martialStatus = MartialStatusSource;
    this.source = InformationSource;
    this.notifyMeBefore = notifyMeBefore;
    this.salesActivities = SaleActivity;
    this.leadCloseRate = LeadCloseRate;
    this._propertyTypeService.asyncFinds();

    this._nameConventionService.register({
      sourceName: this.person.get('Name'),
      fristName: this.person.get('FirstName'),
      lastName: this.person.get('LastName'),
      isLastNameFirst: this.person.get('IsLastNameFirst'),
    })

    this._nameConventionService.watch();

    this.person.get('Name').setValidators([Validators.required]);
    this.lead.get('Source').setValidators([Validators.required]);
    this.lead.get('PropertyTypeSK').setValidators([Validators.required]);
    this.lead.get('SalesActivity').setValidators([Validators.required]);
    this.followup.get('StartDate').setValidators([Validators.required]);
  }
}
