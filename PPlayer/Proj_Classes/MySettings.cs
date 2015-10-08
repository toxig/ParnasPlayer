using System;
using System.Configuration;

namespace PPlayer
{
    public class MySettings : ApplicationSettingsBase
    {

        /// <summary>
        /// Признак необходимости обновление настроек (один раз после обновления)
        /// </summary>
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Необходимо обновление настроек")]
        [global::System.Configuration.DefaultSettingValue("true")]
        public bool p_update_settings
        {
            get { return ((bool)(this["p_update_settings"])); }
            set { this["p_update_settings"] = value; }
        }

        /// <summary>
        /// Проверять обновления при запуске программы
        /// </summary>
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Проверять обновления при запуске программы")]
        [global::System.Configuration.DefaultSettingValue("true")]
        public bool p_check_updates
        {
            get { return ((bool)(this["p_check_updates"])); }
            set { this["p_check_updates"] = value; }
        }

		/// <summary>
        /// Отступ от края в полноэкранном режиме
        /// </summary>
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Отступ от края в полноэкранном режиме")]
        [global::System.Configuration.DefaultSettingValue("10")]
        public int p_FullScreen_Delta 
        {
            get { return ((int)(this["p_FullScreen_Delta"])); }
            set { this["p_FullScreen_Delta"] = value; }            
        }

        /// <summary>
        /// Размер шрифта для плейлиста
        /// </summary>
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Размер шрифта для плейлиста")]        
        [global::System.Configuration.DefaultSettingValue("13.8")]
        public float p_PL_FontSize
        {
            get
            {
                return ((float)(this["p_PL_FontSize"]));
            }
            set
            {
                this["p_PL_FontSize"] = value;
            }
        }

        /// <summary>
        /// Список открытых плейлистов
        /// </summary>
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Открытые плейлисты")]        
        [global::System.Configuration.DefaultSettingValue("")]
        public string p_PL_OpenFiles
        {
            get
            {
                return ((string)this["p_PL_OpenFiles"]);
            }
            set
            {
                this["p_PL_OpenFiles"] = value;
            }
        }

        /// <summary>
        /// Показать эквалайзер
        /// </summary>
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Показать эквалайзер")]        
        [global::System.Configuration.DefaultSettingValue("false")]
        public bool p_bar_show_EQ
        {
            get
            {
                return ((bool)this["p_bar_show_EQ"]);
            }
            set
            {
                this["p_bar_show_EQ"] = value;
            }
        }

        /// <summary>
        /// Показать кнопки воспроизведения
        /// </summary>
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Показать кнопки воспроизведения")]        
        [global::System.Configuration.DefaultSettingValue("true")]
        public bool p_bar_show_PLAY
        {
            get
            {
                return ((bool)this["p_bar_show_PLAY"]);
            }
            set
            {
                this["p_bar_show_PLAY"] = value;
            }
        }

        /// <summary>
        /// Полноэкранный режим
        /// </summary>
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Полноэкранный режим")]        
        [global::System.Configuration.DefaultSettingValue("false")]
        public bool p_Form_Maximized
        {
            get
            {
                return ((bool)this["p_Form_Maximized"]);
            }
            set
            {
                this["p_Form_Maximized"] = value;
            }
        }

        /// <summary>
        /// Активный плейлист
        /// </summary>
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Активный плейлист")]        
        [global::System.Configuration.DefaultSettingValue("1")]
        public int p_PL_ActiveListID
        {
            get
            {
                return ((int)this["p_PL_ActiveListID"]);
            }
            set
            {
                this["p_PL_ActiveListID"] = value;
            }
        }

        /// <summary>
        /// Ширина плейлиста
        /// </summary>
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Ширина плейлиста")]        
        [global::System.Configuration.DefaultSettingValue("380")]
        public int p_PL_PanelWidth
        {
            get
            {
                return ((int)(this["p_PL_PanelWidth"]));
            }
            set
            {
                this["p_PL_PanelWidth"] = value;
            }
        }

        /// <summary>
        /// Папка плейлистов
        /// </summary>
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Папка плейлистов")]        
        [global::System.Configuration.DefaultSettingValue("")]
        public string p_DefFolder_PList
        {
            get
            {
                return ((string)this["p_DefFolder_PList"]);
            }
            set
            {
                this["p_DefFolder_PList"] = value;
            }
        }

        /// <summary>
        /// Папка текстов
        /// </summary>
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Папка текстов")]        
        [global::System.Configuration.DefaultSettingValue("")]
        public string p_DefFolder_Texts
        {
            get
            {
                return ((string)this["p_DefFolder_Texts"]);
            }
            set
            {
                this["p_DefFolder_Texts"] = value;
            }
        }

        /// <summary>
        /// Папка музыки
        /// </summary>
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Папка музыки")]        
        [global::System.Configuration.DefaultSettingValue("")]
        public string p_DefFolder_Music
        {
            get
            {
                return ((string)this["p_DefFolder_Music"]);
            }
            set
            {
                this["p_DefFolder_Music"] = value;
            }
        }

        /// <summary>
        /// Нонстоп плейлиста
        /// </summary>
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Нонстоп плейлиста")]        
        [global::System.Configuration.DefaultSettingValue("false")]
        public bool p_Repeat_Status
        {
            get
            {
                return ((bool)this["p_Repeat_Status"]);
            }
            set
            {
                this["p_Repeat_Status"] = value;
            }
        }

        /// <summary>
        /// Фейдер время
        /// </summary>
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Фейдер треков")]        
        [global::System.Configuration.DefaultSettingValue("2")]
        public int p_Fade_time
        {
            get
            {
                return ((int)this["p_Fade_time"]);
            }
            set
            {
                this["p_Fade_time"] = value;
            }
        }

        /// <summary>
        /// Чтение тэгов при загрузке
        /// </summary>
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Чтение тэгов при загрузке")]
        [global::System.Configuration.DefaultSettingValue("false")]
        public bool p_Check_Tags
        {
            get
            {
                return ((bool)this["p_Check_Tags"]);
            }
            set
            {
                this["p_Check_Tags"] = value;
            }
        }

        
        /// <summary>Проверка наличия файлов на диске</summary>        
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Проверка наличия файлов на диске")]
        [global::System.Configuration.DefaultSettingValue("true")]
        public bool p_Check_Exist
        {
            get
            {
                return ((bool)this["p_Check_Exist"]);
            }
            set
            {
                this["p_Check_Exist"] = value;
            }
        }

        /// <summary>
        /// Высота горячего списка
        /// </summary>
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Высота горячего списка")]
        [global::System.Configuration.DefaultSettingValue("150")]
        public int p_HotList_Height
        {
            get
            {
                return ((int)this["p_HotList_Height"]);
            }
            set
            {
                this["p_HotList_Height"] = value;
            }
        }

        /// <summary>
        /// Позиция горячего списка - в табах или сверху
        /// </summary>
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Позиция горячего списка")]
        [global::System.Configuration.DefaultSettingValue("false")]
        public bool p_HotList_Position
        {
            get
            {
                return ((bool)this["p_HotList_Position"]);
            }
            set
            {
                this["p_HotList_Position"] = value;
            }
        }

        /// <summary>
        /// Версия программы (версия настроек)
        /// </summary>
        [global::System.Configuration.UserScopedSetting()]
        [global::System.Configuration.SettingsDescription("Папка плейлистов")]
        [global::System.Configuration.DefaultSettingValue("")]
        public string p_App_Version
        {
            get
            {
                return ((string)this["p_App_Version"]);
            }
            set
            {
                this["p_App_Version"] = value;
            }
        }
    }
}