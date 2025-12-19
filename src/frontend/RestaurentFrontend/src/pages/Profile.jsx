import React from 'react'
import { Profile as ProfileComponent } from "../components/index"

function Profile() {
  return (
    <div className="min-h-[calc(100vh-64px)] flex items-center justify-center py-12">
      <div className="w-full max-w-4xl">
        <ProfileComponent/>
      </div>
    </div>
  )
}

export default Profile