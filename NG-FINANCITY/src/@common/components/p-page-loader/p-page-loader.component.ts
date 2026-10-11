import { Component } from '@angular/core';
import { PageLoaderService } from './p-page-loader.service';

@Component({
  selector: 'page-loader',
  templateUrl: './p-page-loader.component.html',
  styleUrls: ['./p-page-loader.component.scss'],
  providers: [PageLoaderService]
})
export class PageLoaderComponent {
  constructor(public loaderService: PageLoaderService) { }
}
