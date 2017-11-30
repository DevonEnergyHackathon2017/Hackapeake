import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { MainlistComponent } from './mainlist/mainlist.component';
import { PropertyDetailComponent } from './property-detail/property-detail.component';


@NgModule({
  declarations: [
    AppComponent,
    MainlistComponent,
    PropertyDetailComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    RouterModule.forRoot([{
      path: 'property-detail/:id',
      component: PropertyDetailComponent
    },
    {
      path: 'home',
      component: MainlistComponent
    },
    {
      path: '',
      redirectTo: '/home',
      pathMatch: 'full'
    }
  ])
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
