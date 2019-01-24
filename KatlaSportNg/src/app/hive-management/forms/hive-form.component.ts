import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HiveService } from '../services/hive.service';
import { Hive } from '../models/hive';
import { ErrorHandlerService } from '../services/error-handler.service';

@Component({
  selector: 'app-hive-form',
  templateUrl: './hive-form.component.html',
  styleUrls: ['./hive-form.component.css']
})
export class HiveFormComponent implements OnInit {

  hive = new Hive(0, "", "", "", false, "");
  existed = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private hiveService: HiveService,
    private errorService: ErrorHandlerService
    ) { }

  ngOnInit() {
    this.route.params.subscribe(p => {
      if (p['id'] === undefined) return;
      this.hiveService.getHive(p['id']).subscribe(h => this.hive = h, error => alert(this.errorService.handleError(error)));
      this.existed = true;
    });
  }

  navigateToHives() {
    this.router.navigate(['/hives']);
  }

  onCancel() {
    this.navigateToHives();
  }
  
  onSubmit() {
    if (this.existed){
      this.hiveService.updateHive(this.hive)
                      .subscribe(() => this.navigateToHives(), error => alert(this.errorService.handleError(error)));
    }
    else{
      this.hiveService.addHive(this.hive)
                      .subscribe(() => this.navigateToHives(), error => alert(this.errorService.handleError(error)));
    }
  }

  onDelete() {
    this.setStatus(true);
  }

  onUndelete() {
   this.setStatus(false);
  }

  onPurge() {
    if (confirm("Are you sure to delete the hive - " + this.hive.name + " ?")){
      this.hiveService.deleteHive(this.hive.id)
                    .subscribe(() => {this.navigateToHives()}, error => alert(this.errorService.handleError(error)));
    }
  }

  private setStatus(isDeleted: boolean ){
    this.hiveService.setHiveStatus(this.hive.id, isDeleted)
                    .subscribe(() => this.hive.isDeleted = isDeleted, error => alert(this.errorService.handleError(error)));
  }
}
