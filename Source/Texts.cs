using System;

namespace TelegramBotGDev
{
    public class Texts
    {
        public static string ACCEPT = "Aceptar";
        public static string CANCEL = "Cancelar";
        public static string CONFIRM_PUBLICATION = "Pulsa *aceptar* para enviar la publicación. Pulsa *cancelar* para cancelar la publicación.";
        public static string ADD_USER_PROBLEM = "Lo siento, ha habido un problema. Vuelve a intentarlo en unos segundos.";
        public static string NOT_UNDERSTOOD = "No te entiendo muy bien. Por favor, pulsa *aceptar* para enviar la publicación. Pulsa *cancelar* para cancelar la publicación.";
        public static string WELCOME = "¡Bienvenid@! Envía aquello que quieras compartir y será publicado en @recursosgamedev. Puedes realizar 3 publicaciones por hora. Ahora mismo sólo acepto texto (enlaces, etc.), pero no imágenes, audios, gifs...\n\nPara cualquier consulta, problema o sugerencia habla con @Delunado. Y si estás interesado en el Game Dev, no dudes en unirte a @spaingamedevs.";
        public static string HELP = "Puedes realizar 3 publicaciones por hora. Ahora mismo sólo acepto texto (enlaces, etc.), pero no imágenes, audios, gifs...\n\nSi a veces tardo en responder, no te preocupes. El servidor en el que habito es muy humilde, ¡pero te atenderé pronto! Para cualquier consulta, problema o sugerencia habla con @Delunado.";

        public static string MaxPublicationsReachedText(int minutesToWait)
        {
            return $"Lo siento, has alcanzado el máximo de publicaciones enviadas. ¡Vuelve en {minutesToWait} minutos!";
        }
        
        public static string RemainingPublicationsText(int remainingPublications, bool publicationAccepted)
        {
            if (publicationAccepted)
                return $"¡Gracias por compartir! Puedes realizar {remainingPublications} publicación(es) más hoy.";
            else
                return $"Publicación cancelada. Puedes realizar {remainingPublications} publicación(es) más hoy.";
        }
    }
}
