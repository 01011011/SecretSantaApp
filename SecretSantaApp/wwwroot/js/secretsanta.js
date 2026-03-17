document.addEventListener('alpine:init', () => {
    Alpine.data('secretSanta', () => ({
        users: [],
        groups: [],
        matches: [],
        selectedUsers: [],
        userToAdd: '',
        groupName: '',
        showGroupNameInput: false,
        selectedGroupId: '',

        baseUriUsers: 'api/Users',
        baseUriGroups: 'api/Groups',

        get usersVisible() { return this.users.length > 0; },
        get groupsVisible() { return this.groups.length > 0; },
        get checkVisible() { return this.users.length > 1; },
        get enableGroupAdd() { return this.selectedUsers.length > 1; },
        get canAddToGroup() { return this.selectedUsers.length > 0 && this.groupsVisible; },
        get showBoth() { return this.canAddToGroup && this.enableGroupAdd; },
        get showMatching() { return this.matches.length > 0; },

        async init() {
            await this.getUserData();
            await this.getGroupData();
        },

        isUserInGroup(userId) {
            return this.groups.some(g => g.Users.some(u => u.Id === userId));
        },

        isUserSelected(userId) {
            return this.selectedUsers.some(u => u.Id === userId);
        },

        toggleUserSelection(user) {
            const idx = this.selectedUsers.findIndex(u => u.Id === user.Id);
            if (idx >= 0) {
                this.selectedUsers.splice(idx, 1);
            } else {
                this.selectedUsers.push({ Id: user.Id, Name: user.Name, InGroup: user.InGroup });
            }
        },

        async getUserData() {
            try {
                const response = await fetch(this.baseUriUsers);
                if (response.ok) {
                    this.users = await response.json();
                }
            } catch (e) {
                alert('Error loading users');
            }
        },

        async addUser() {
            if (!this.userToAdd) return;
            try {
                const response = await fetch(this.baseUriUsers, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(this.userToAdd)
                });
                if (response.ok) {
                    this.userToAdd = '';
                    await this.getUserData();
                } else {
                    const msg = await response.text();
                    alert(msg);
                }
            } catch (e) {
                alert('Error adding user');
            }
        },

        async deleteUser(user) {
            this.selectedUsers = this.selectedUsers.filter(u => u.Id !== user.Id);
            try {
                await fetch(`${this.baseUriUsers}/?id=${user.Id}`, { method: 'DELETE' });
                await this.getUserData();
            } catch (e) {
                alert('Error deleting user');
            }
        },

        async getGroupData() {
            try {
                const response = await fetch(this.baseUriGroups);
                if (response.ok) {
                    this.groups = await response.json();
                }
            } catch (e) {
                alert('Error loading groups');
            }
        },

        async saveGroup() {
            if (!this.groupName) return;
            const group = { Id: null, Name: this.groupName, Users: this.selectedUsers };
            try {
                const response = await fetch(this.baseUriGroups, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(group)
                });
                if (response.ok) {
                    this.groupName = '';
                    this.selectedUsers = [];
                    this.showGroupNameInput = false;
                    await this.getGroupData();
                    await this.getUserData();
                } else {
                    const msg = await response.text();
                    alert(msg);
                }
            } catch (e) {
                alert('Error saving group');
            }
        },

        async updateGroup() {
            if (!this.selectedGroupId) return;
            try {
                const response = await fetch(`${this.baseUriGroups}/?id=${this.selectedGroupId}`, {
                    method: 'PUT',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(this.selectedUsers)
                });
                if (response.ok) {
                    this.selectedUsers = [];
                    this.selectedGroupId = '';
                    await this.getGroupData();
                    await this.getUserData();
                } else {
                    const msg = await response.text();
                    alert(msg);
                }
            } catch (e) {
                alert('Error updating group');
            }
        },

        async removeFromGroup(groupId, user) {
            const group = this.groups.find(g => g.Id === groupId);
            if (group && group.Users.length <= 2) {
                if (!confirm('Each group must have at least 2 people. Removing this person will delete the whole group. Continue?')) {
                    return;
                }
            }
            try {
                await fetch(`${this.baseUriGroups}/?id=${groupId}`, {
                    method: 'DELETE',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify(user)
                });
                await this.getGroupData();
                await this.getUserData();
            } catch (e) {
                alert('Error removing from group');
            }
        },

        async runMatch() {
            try {
                const response = await fetch(`${this.baseUriUsers}/Match`);
                const data = await response.json();
                if (!response.ok || !data.length) {
                    alert('Not possible matching!!!');
                } else {
                    this.matches = data;
                }
            } catch (e) {
                alert('Error running match');
            }
        }
    }));
});