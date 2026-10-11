import { Component, OnInit, Input } from '@angular/core';
import { SalutationSource, GenderSource, MartialStatusSource } from 'src/@common/common.constant';
import { NameConventionService } from 'src/app/services/name-conventions.service';
import { FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'fc-client',
  templateUrl: './client.page.html',
  providers: [NameConventionService]
})
export class ClientPage implements OnInit {

  @Input() form: FormGroup;

  public salutation = [];
  public gender = [];
  public martialStatus = [];

  constructor(private _nameConventionService: NameConventionService) {
  }

  ngOnInit() {

    this.salutation = SalutationSource;
    this.gender = GenderSource;
    this.martialStatus = MartialStatusSource;

    this._nameConventionService.register({
      sourceName: this.form.get('Name'),
      fristName: this.form.get('FirstName'),
      lastName: this.form.get('LastName'),
      isLastNameFirst: this.form.get('IsLastNameFirst'),
    })

    this._nameConventionService.watch();

    this.form.get('Name').setValidators([Validators.required]);
    this.form.get('NRIC').setValidators([Validators.required]);
  }
}
