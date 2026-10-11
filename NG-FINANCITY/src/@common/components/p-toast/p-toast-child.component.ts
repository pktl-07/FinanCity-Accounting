import { Component } from '@angular/core';
@Component({
  selector: 'p-toast-child',
  template: `
      <div style="font-size:1.2rem;">
        {{ message }}
      </div>
  `
})
export class PToastChildComponent {

  message: string;
  constructor() { }
}

