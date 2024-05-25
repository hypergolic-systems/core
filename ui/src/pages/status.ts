import {Component, computed, inject, input} from '@angular/core';
import { KspClient, Part, Vessel } from '../ksp/client';
import { JsonPipe } from '@angular/common';
import { ClrDatagridModule } from '@clr/angular';

interface ElectricalPart {
  id: string;
  kind: string;
  flow: string;
  canProduce: string;
  canConsume: string;
  storage: string;
}

function maybeElectricalPart(part: Part): ElectricalPart|undefined {
  for (const module of part.modules ?? []) {
    for (const component of module.virtualComponents ?? []) {
      switch (component.type) {
        case  'Battery':
          return {
            id: part.id,
            kind: 'Battery',
            flow: `${component.flow} W`,
            storage: `${component.stored} / ${component.capacity} (J)`,
            canProduce: `${component.canProduce} W`,
            canConsume: `${component.canConsume} W`,
          };
        case 'RadioisotopeThermalGenerator':
          return {
            id: part.id,
            kind: 'RTG',
            flow: `${component.flow} W`,
            canProduce: `${component.canProduce} W`,
            canConsume: `n/a`,
            storage: `n/a`,
          };
      }
    }
  }
  return undefined;
}

@Component({
  standalone: true,
  selector: 'hgui-vessel-display',
  imports: [ClrDatagridModule],
  template: `
    <clr-datagrid>
      <clr-dg-column>Kind</clr-dg-column>
      <clr-dg-column>Flow</clr-dg-column>
      <clr-dg-column>Max Production</clr-dg-column>
      <clr-dg-column>Max Consumption</clr-dg-column>
      <clr-dg-column>Storage</clr-dg-column>

      @for (part of electricalParts(); track part.id) {
        <clr-dg-row>
          <clr-dg-cell>{{ part.kind }}</clr-dg-cell>
          <clr-dg-cell>{{ part.flow }}</clr-dg-cell>
          <clr-dg-cell>{{ part.canProduce }}</clr-dg-cell>
          <clr-dg-cell>{{ part.canConsume }}</clr-dg-cell>
          <clr-dg-cell>{{ part.storage }}</clr-dg-cell>
        </clr-dg-row>
      }
    </clr-datagrid>
  `
})
export class VesselDisplay {
  vessel = input.required<Vessel>();
  electricalParts = computed(() =>  this.vessel().parts?.map(maybeElectricalPart).filter(p => p !== undefined) as ElectricalPart[]|undefined ?? []);
}

@Component({
  standalone: true,
  imports: [JsonPipe, VesselDisplay],
  template: `
    @for (vessel of vessels(); track vessel.id) {
      <h2>{{ vessel.name}}</h2>
      <hgui-vessel-display [vessel]="vessel" />
    }
  `,
})
export class StatusRoute {

  vessels = inject(KspClient).vessels;

}