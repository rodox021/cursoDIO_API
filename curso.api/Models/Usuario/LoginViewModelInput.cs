using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace curso.api.Models.Usuario
{
    public class LoginViewModelInput
    {
        [Required(ErrorMessage = "O Campo {0} é Obrigatório")]
        public string Login { get; set; }
        [Required(ErrorMessage = "O Campo {0} é Obrigatório")]
        public string Senha { get; set; }

    }
}
