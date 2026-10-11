import { Component, Input } from '@angular/core';
import { CommissionReceivedAllocationService } from 'src/@sales/services/commission-received-allocation.service';
import { VendorService } from 'src/@sales/services/vendor.service';
import { SalespersonManagementService } from 'src/@sales/services/salesperson-management.service';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-commission-payable-detail-salesperson',
  templateUrl: './commission-payable-detail-salesperson.component.html',
  providers: [
    SalespersonManagementService,
    VendorService,
    CommissionReceivedAllocationService
  ]
})
export class CommissionPayableDetailSalespersonComponent {

  @Input() formGroup: FormGroup;
}
