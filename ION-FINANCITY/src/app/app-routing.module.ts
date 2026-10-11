import { NgModule } from '@angular/core';
import { PreloadAllModules, RouterModule } from '@angular/router';
import { AuthGuard } from 'src/@common/Infrastructure/auth.guard';
import { ClientResolverService } from './services/client-resolver.service';
import { ExternalSalespersonResolverService } from './services/external-salesperson.-resolver.service';
import { MyProfileResolverService } from './services/my-profile-resolver.service';
import { PropertyTransactionResolverService } from './services/property-transaction.-resolver.service';
import { VendorResoverService } from './services/vendor-resolver.service';
import { MyCommissionResolverService } from './services/my-commission-resolver.service';
import { LeadResolverService } from './services/lead-resolver.service';

const routes: any = [
  { path: '', redirectTo: 'home', pathMatch: 'full', display: 'none', canActivate: [AuthGuard] },
  { path: 'home', title: 'Home', loadChildren: () => import('./pages/home/home.module').then(m => m.HomePageModule), icon: 'home', canActivate: [AuthGuard] },
  { path: 'property-transaction-list', title: 'Transaction', loadChildren: () => import('./pages/property-transaction-list/property-transaction-list.module').then(m => m.PropertyTransactionListPageModule), color: 'favorite', icon: 'money', canActivate: [AuthGuard] },
  { path: 'property-transaction-detail', title: 'Transaction', resolve: { propertyTransaction: PropertyTransactionResolverService }, loadChildren: () => import('./pages/property-transaction-detail/property-transaction-detail.module').then(m => m.PropertyTransactionDetailPageModule), icon: 'money', display: 'none', canActivate: [AuthGuard] },
  { path: 'property-transaction-detail/:id', title: 'Transaction', resolve: { propertyTransaction: PropertyTransactionResolverService }, loadChildren: () => import('./pages/property-transaction-detail/property-transaction-detail.module').then(m => m.PropertyTransactionDetailPageModule), icon: 'money', display: 'none', canActivate: [AuthGuard] },
  { path: 'my-commission-list', title: 'My Commission', loadChildren: () => import('./pages/my-commission-list/my-commission.module').then(m => m.MyCommissionListPageModule), icon: 'chart', color: 'warning', canActivate: [AuthGuard] },
  { path: 'my-commission-detail/:id', title: 'My Commission', resolve: { commission: MyCommissionResolverService }, loadChildren: './pages/my-commission-detail/my-commission-detail.module#MyCommissionDetailPageModule', icon: 'chart', color: 'medium', display: 'none', canActivate: [AuthGuard] },
  { path: 'client-contact-list', title: 'Client Contacts', loadChildren: () => import('./pages/client-contact-list/client-contact-list.module').then(m => m.ClientContactListPageModule), icon: 'client', color: 'success', canActivate: [AuthGuard] },
  { path: 'client-contact-detail', title: 'Client Contacts', resolve: { client: ClientResolverService }, loadChildren: () => import('./pages/client-contact-detail/client-contact-detail.module').then(m => m.ClientContactDetailPageModule), icon: 'client', display: 'none', canActivate: [AuthGuard] },
  { path: 'client-contact-detail/:id', title: 'Client Contacts', resolve: { client: ClientResolverService }, loadChildren: () => import('./pages/client-contact-detail/client-contact-detail.module').then(m => m.ClientContactDetailPageModule), icon: 'client', display: 'none', canActivate: [AuthGuard] },
  { path: 'lead-contact-list', title: 'Lead Contacts', loadChildren: () => import('./pages/lead-contact-list/lead-contact-list.module').then(m => m.LeadContactListPageModule), icon: 'lead', color: 'success', canActivate: [AuthGuard] },
  { path: 'lead-contact-detail', title: 'Lead Contacts', resolve: { client: LeadResolverService }, loadChildren: () => import('./pages/lead-contact-detail/lead-contact-detail.module').then(m => m.LeadContactDetailPageModule), icon: 'lead', display: 'none', canActivate: [AuthGuard] },
  { path: 'lead-contact-detail/:id', title: 'Lead Contacts', resolve: { client: LeadResolverService }, loadChildren: () => import('./pages/lead-contact-detail/lead-contact-detail.module').then(m => m.LeadContactDetailPageModule), icon: 'lead', display: 'none', canActivate: [AuthGuard] },
  { path: 'vendor-contact-list', title: 'Vendor Contacts', loadChildren: () => import('./pages/vendor-contact-list/vendor-contact-list.module').then(m => m.VendortListPageModule), icon: 'building', color: 'darkyellow', canActivate: [AuthGuard] },
  { path: 'vendor-contact-detail', title: 'Vendor Contacts', resolve: { vendor: VendorResoverService }, loadChildren: () => import('./pages/vendor-contact-detail/vendor-contact-detail.module').then(m => m.VendorContactDetailPageModule), icon: 'building', display: 'none', canActivate: [AuthGuard] },
  { path: 'vendor-contact-detail/:id', title: 'Vendor Contacts', resolve: { vendor: VendorResoverService }, loadChildren: () => import('./pages/vendor-contact-detail/vendor-contact-detail.module').then(m => m.VendorContactDetailPageModule), icon: 'building', display: 'none', canActivate: [AuthGuard] },
  { path: 'cobroker-contact-list', title: 'Co-Broke Contacts', loadChildren: () => import('./pages/cobroker-contact-list/cobroker-contact-list.module').then(m => m.CoBrokerContactListPageModule), icon: 'external-salesperson', color: 'danger', canActivate: [AuthGuard] },
  { path: 'cobroker-contact-detail', title: 'Co-Broker Contacts', resolve: { externalSalsperson: ExternalSalespersonResolverService }, loadChildren: () => import('./pages/cobroker-contact-detail/cobroker-contact-detail.module').then(m => m.CoBrokerContactDetailPageModule), icon: 'external-salesperson', display: 'none', canActivate: [AuthGuard] },
  { path: 'cobroker-contact-detail/:id', title: 'Co-Broker Contacts', resolve: { externalSalsperson: ExternalSalespersonResolverService }, loadChildren: () => import('./pages/cobroker-contact-detail/cobroker-contact-detail.module').then(m => m.CoBrokerContactDetailPageModule), icon: 'external-salesperson', display: 'none', canActivate: [AuthGuard] },
  { path: 'salesperson-profile', title: 'My Profile', resolve: { profile: MyProfileResolverService }, loadChildren: () => import('./pages/my-profile/my-profile.module').then(m => m.MyProfilePageModule), icon: 'profile', color: 'medium', canActivate: [AuthGuard] },
  { path: 'agent-tool', title: 'Agent Tools', loadChildren: () => import('./pages/agent-tool/agent-tool.module').then(m => m.AgentToolPageModule), icon: 'tool', color: 'darkyellow', canActivate: [AuthGuard] },
  { path: 'change-password', title: 'Change My Password', loadChildren: () => import('./pages/auth/change-password/change-password.module').then(m => m.ChangePasswordPageModule), icon: 'key', canActivate: [AuthGuard] },
  { path: 'login', loadChildren: () => import('./pages/auth/login/login.module').then(m => m.LoginPageModule), display: 'none' },
  { path: 'reset', loadChildren: () => import('./pages/auth/reset/reset.module').then(m => m.ResetPageModule), display: 'none' },
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, { preloadingStrategy: PreloadAllModules }),
  ],
  exports: [RouterModule],
  providers: []
})
export class AppRoutingModule { }
