import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail-resolver';
import { MemberListResolver } from './_resolvers/member-list-resolver';

// Ordering is IMPORTANT!!! resolved sequentially
export const appRoutes: Routes = [
    { path: '', component: HomeComponent }, // covers page http://localhost:4200
    {
        path: '', // dummy root which will result in concat parent path and child path
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'members', component: MemberListComponent,
                resolve: {users: MemberListResolver}},
            { path: 'members/:id', component: MemberDetailComponent,
                resolve: {user: MemberDetailResolver} },
            { path: 'lists', component: ListsComponent},
            { path: 'messages', component: MessagesComponent },
        ]
    },
    { path: '**', redirectTo: '', pathMatch: 'full' }
];

