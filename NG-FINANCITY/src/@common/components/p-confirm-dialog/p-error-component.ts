import { Component } from '@angular/core';

@Component({
  selector: 'p-error-component',
  template: `
  <div class="container-fluid">
    <ul>
      <li *ngFor="let error of errors" class="text-danger">{{ error }}</li>
    </ul>
  </div>`
})
export class PErrorComponent {

  private _errors = [];
  public get errors() {
    return this._errors;
  }
  public set errors(value) {
    this._errors = value;
  }

  constructor() {
  }
}

@Component({
  selector: 'p-warning-new',
  template: `
  <div class="container-fluid">
   <span style="display:block">Are you sure you want to create new record.</span>
   <span style="display:block">Any unsave data will be <span style="color:var(--danger);font-weight:bold">Lost</span>.</span>
  </div>`
})
export class PWarningCreateComponent {
}

@Component({
  selector: 'p-validation-fail',
  template: `
  <div class="container-fluid">
   <span style="display:block">Validation has <span style="color:var(--danger);font-weight:bold">Failed</span>.</span>
   <span style="display:block">Please check through the form again.</span>
  </div>`
})
export class PValidationFailComponent {
}

@Component({
  selector: 'p-file-warnning-delte',
  template: `
  <div class="container-fluid">
   <span style="display:block">Are you sure you want to delete.</span>
   <span style="display:block">The deleted file can not be <span style="color:var(--danger);font-weight:bold">restored</span> in the futrue.</span>
  </div>`
})
export class PFileDeletWarnningComponent {
}

