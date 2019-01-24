import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HiveSectionService } from '../services/hive-section.service';
import { ErrorHandlerService } from '../services/error-handler.service';
import { HiveSection } from '../models/hive-section';

@Component({
  selector: 'app-hive-section-form',
  templateUrl: './hive-section-form.component.html',
  styleUrls: ['./hive-section-form.component.css']
})
export class HiveSectionFormComponent implements OnInit {

  hiveSection = new HiveSection(0, "", "", 0, false, "");
  existed = false;
  storeHiveId: number;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private hiveSectionService: HiveSectionService,
    private errorService : ErrorHandlerService
  ) { }

  ngOnInit() {
    this.route.params.subscribe( p => {
      this.storeHiveId = p['hiveId'];
      if(p['id'] === undefined) return;
      this.hiveSectionService.getHiveSection(p['id']).subscribe(s => this.hiveSection = s, error => alert(this.errorService.handleError(error)));
      this.existed=true;
    });
  }

  onCancel(){
    this.navigateToSections();
  }

  onSubmit(){
    this.hiveSection.storeHiveId = this.storeHiveId;
    if(this.existed){
      this.hiveSectionService.updateHiveSection(this.hiveSection).subscribe(() => this.navigateToSections(), error => alert(this.errorService.handleError(error)));
    }
    else{
      this.hiveSectionService.addHiveSection(this.hiveSection).subscribe(() => this.navigateToSections(), error => alert(this.errorService.handleError(error)));
    }
  }

  onDelete(){
    this.setStatus(true);
  }

  onUndelete(){
    this.setStatus(false);
  }

  onPurge(){
    if (confirm("Are you sure to delete the section - " + this.hiveSection.name + " ?")){
      this.hiveSectionService.deleteHiveSection(this.hiveSection.id).subscribe(() => this.navigateToSections(), error => alert(this.errorService.handleError(error)));
    }
  }

  private navigateToSections(){
    this.router.navigate([`/hive/${this.storeHiveId}/sections`]);
  }

  private setStatus(isDeleted: boolean){
    this.hiveSectionService.setHiveSectionStatus(this.hiveSection.id, isDeleted)
                            .subscribe(() => this.hiveSection.isDeleted = isDeleted, error => alert(this.errorService.handleError(error)));
  }

}
