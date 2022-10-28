﻿using System;
using System.Collections.Generic;

namespace SocialMedia.Core.DTOs
{
    public  class UserDto 
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Telephone { get; set; }
        public bool IsActive { get; set; }

     
    }
}
