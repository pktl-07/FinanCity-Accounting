import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { PGridViewComponent } from 'src/@common/components/p-grid-view/p-grid-view.component';
import { LoggerService } from 'src/@sales/services/logger.service';
import { kendoFilter } from 'src/@common/service/utility-service';

@Component({
  selector: 'app-salesperson-career-activity',
  templateUrl: './salesperson-career-activity.component.html',
  providers: [
    LoggerService
  ]
})
export class CareerActivityComponent implements OnInit {

  @Input() public SPSK: string;
  @ViewChild('careerActivityGrid', { static: true }) _gridView: PGridViewComponent;

  constructor(public fb: FormBuilder,
    public _loggerService: LoggerService) {
  }

  ngOnInit(): void {

    this._gridView.config = {
      key: 'SysKey',
      filterable: 'none',
      fetch: this._loggerService,
      columns: [
        { dataType: 'date', field: 'CreatedDate', filter: false, title: 'Modified Date', width: 240 },
        { dataType: 'string', field: 'OldValue', filter: false, title: 'Previous', width: 240 },
        { dataType: 'string', field: 'NewValue', filter: false, title: 'New', width: 120 },
        { dataType: 'string', field: 'Description', filter: false, title: 'Description' },
        { dataType: 'string', field: 'ModifiedUser', filter: false, title: 'Modified User', width: 240 },
      ]
    };

    this._loggerService.asyncFinds({
      filter: {
        logic: 'and',
        filters: kendoFilter([
          { field: 'RefSK', value: this.SPSK, operator: 'eq', logic: 'and' }
        ])
      }
    });
  }
}
