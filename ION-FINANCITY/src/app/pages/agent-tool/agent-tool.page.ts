import { Component } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { UtilityService } from 'src/app/services/util.service';

@Component({
  selector: 'fc-agent-tool.page',
  templateUrl: './agent-tool.page.html',
  providers: [UtilityService]
})
export class AgentToolPage {

  public form: FormGroup;
}
