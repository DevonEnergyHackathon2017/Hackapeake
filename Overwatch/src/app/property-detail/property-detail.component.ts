import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { OnDestroy } from '@angular/core/src/metadata/lifecycle_hooks';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-property-detail',
  templateUrl: './property-detail.component.html',
  styleUrls: ['./property-detail.component.css']
})
export class PropertyDetailComponent implements OnInit, OnDestroy {
  id: number;
  private sub: any;
  detail: any;
  latestAlertDescription: string;
  imageUrl: string;

  constructor(private route: ActivatedRoute, private httpClient: HttpClient) { }

  ngOnInit() {
    this.sub = this.route.params.subscribe(params => {
       this.id = +params['id'];

       this.httpClient.get('http://localhost:7071/api/PropertyRetrieval?propertyId=' + this.id).subscribe(data => {
        this.detail = data;
        this.latestAlertDescription = this.detail.latestPropertyAlert.propertyAlert;
        this.imageUrl = this.detail.latestPropertyAlert.imageUrl;
        console.log(this.detail);
      });
    });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

}
