using System;
using System.Collections.Generic;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;

namespace PPlayer
{
    /// <summary>
    /// Параметры воспроизведения трека
    /// </summary>
    public class StreamClass
    {
        // конструктор состояния потока
        private StreamStatus _v_stream_status = StreamStatus.FREE;
        public bool v_streem_need_free = true;
        private int _v_FadeStopTime = 0;
        private float _v_stream_volume = 0;

        public float v_stream_volume
        {
            get
            {
                return _v_stream_volume;
            }
            set
            {
                _v_stream_volume = value;
                Bass.BASS_ChannelSlideAttribute(v_stream, BASSAttribute.BASS_ATTRIB_VOL, _v_stream_volume, 0);
            }
        }

        /// <summary>
        /// Инициализация потока
        /// </summary>
        public void p_Init_NewPlayStream(string fileName, bool? SoundDevice)
        {
            if (SoundDevice.Value)
            {
                // останавливаем старый поток  
                // не подходит для кросфейдера  
                if (v_stream != 0 && v_streem_need_free) v_stream_status = StreamStatus.FREE;

                // новый поток
                //v_stream = Bass.BASS_StreamCreateFile( fileName, 0, 0, BASSFlag.BASS_DEFAULT);  
                v_stream = Bass.BASS_StreamCreateFile(fileName, 0, 0, BASSFlag.BASS_STREAM_DECODE); // BASSFlag.BASS_DEFAULT
                v_stream = BassFx.BASS_FX_TempoCreate(v_stream, BASSFlag.BASS_FX_FREESOURCE);
                //BASS_StreamCreateFile(FALSE, file, 0, 0, BASS_STREAM_DECODE);
                _v_stream_status = StreamStatus.STOP; // фактическое состояние потока - остановлен
            }
        }
        /// <summary>
        /// Поток
        /// </summary>
        public int v_stream = 0;
        /// <summary>
        /// Состояние потока         
        /// </summary>
        public StreamStatus v_stream_status
        {
            get
            {
                if (Bass.BASS_ChannelIsActive(v_stream) == BASSActive.BASS_ACTIVE_STOPPED
                    && _v_stream_status == StreamStatus.PLAY)
                {
                    _v_stream_status = StreamStatus.END;
                }

                return _v_stream_status;
            }
            set
            {
                if (v_stream == 0)
                {
                    _v_stream_status = StreamStatus.FREE;
                    return;
                }

                if (_v_stream_status != value)
                {
                    Bass.BASS_ChannelLock(v_stream, true);
                    try
                    {
                        switch (value)
                        {
                            case StreamStatus.PLAY:
                                Bass.BASS_ChannelPlay(v_stream, false);
                                v_FadeEnd = false;
                                break;
                            case StreamStatus.END:
                                Bass.BASS_ChannelStop(v_stream);
                                break;
                            case StreamStatus.STOP:
                                Bass.BASS_ChannelStop(v_stream);
                                break;
                            case StreamStatus.PAUSE:
                                Bass.BASS_ChannelPause(v_stream);
                                break;
                            case StreamStatus.FREE:
                                if (_v_stream_status != StreamStatus.END
                                    && _v_stream_status != StreamStatus.STOP)
                                    Bass.BASS_ChannelStop(v_stream);
                                // освобождаем поток
                                Bass.BASS_StreamFree(v_stream);
                                v_stream = 0;
                                break;
                            default:
                                break;
                        }
                    }
                    catch
                    {
                        System.Windows.Forms.MessageBox.Show(String.Format("Error {0}", Bass.BASS_ErrorGetCode()));
                    }

                    Bass.BASS_ChannelLock(v_stream, false);
                    _v_stream_status = value;
                }
            }
        }
        /// <summary>
        /// Необходимость обработки
        /// </summary>
        public bool v_FadeActive = false;
        /// <summary>
        /// Установить состояние потока после окончания фейдера
        /// </summary>
        public StreamStatus v_FadePostAction = StreamStatus.NONE;
        /// <summary>
        /// Остаток времени до старта обработки (msec)
        /// </summary>
        public int v_FadeStopTime
        {
            get { return _v_FadeStopTime; }
            set
            {
                if (value <= 0 && v_FadeActive)
                {
                    v_stream_status = v_FadePostAction;
                    v_FadeActive = false;
                    v_FadeEnd = true;
                    _v_FadeStopTime = 0;
                }
                else
                {
                    _v_FadeStopTime = value;
                    v_FadeEnd = false;
                }
            }
        }
        /// <summary>
        /// Плейлист на котором играет трек
        /// </summary>
        public int v_PL_List_ID_played = 0;
        /// <summary>
        /// Номер трека в плейлисте
        /// </summary>
        public int v_PL_Row_ID = 0;
        /// <summary>
        /// Длина трека в секундах
        /// </summary>
        public int v_play_max
        {
            get { return int.Parse(((Bass.BASS_ChannelBytes2Seconds(v_stream, Bass.BASS_ChannelGetLength(v_stream)).ToString().Split(',', '.'))[0])); }
        }
        /// <summary>
        /// Текущая позиция воспроизведения трека в секундах
        /// </summary>
        public int v_play_pos
        {
            get { return int.Parse(((Bass.BASS_ChannelBytes2Seconds(v_stream, Bass.BASS_ChannelGetPosition(v_stream)).ToString().Split(',', '.'))[0])); }
        }
        /// <summary>
        /// Фейдер канала: volume - до какой громкости (от 0 до 1), time_msec - время в мс
        /// </summary>
        public void p_fade_channel(float volume, int time_msec)
        {
            if (v_stream_status != StreamStatus.PLAY) v_stream_status = StreamStatus.PLAY;
            Bass.BASS_ChannelSlideAttribute(v_stream, BASSAttribute.BASS_ATTRIB_VOL, volume, time_msec);
        }

        public void p_fade_channel(float volume, int time_msec, float volume_start)
        {
            // из нуля
            Bass.BASS_ChannelSlideAttribute(v_stream, BASSAttribute.BASS_ATTRIB_VOL, volume_start, 0);

            if (v_stream_status != StreamStatus.PLAY) v_stream_status = StreamStatus.PLAY;
            Bass.BASS_ChannelSlideAttribute(v_stream, BASSAttribute.BASS_ATTRIB_VOL, volume, time_msec);
        }
        /// <summary>
        /// Муз файл
        /// </summary>
        public string v_FileName = "";
        /// <summary>
        /// Заголовок Муз файла
        /// </summary>
        public string v_FileNameSrt = "";
        /// <summary>
        /// Текстовый файл
        /// </summary>
        public string v_TextFileName = "";
        /// <summary>
        /// Признак запуска фейдера
        /// </summary>
        public bool v_FadeEnd = false;
    }

    //**************************
    //  StreamStatus
    //  состояние воспроизведения трека
    //**************************
    public enum StreamStatus
    {
        /// <summary>
        /// Остановлен
        /// </summary>
        END,
        /// <summary>
        /// Играет
        /// </summary>
        PLAY,
        /// <summary>
        /// На паузе
        /// </summary>
        PAUSE,
        /// <summary>
        /// Остановлен вручную
        /// </summary>
        STOP,
        /// <summary>
        /// Освобождение потока
        /// </summary>
        FREE,
        /// <summary>
        /// Пусто
        /// </summary>
        NONE
    };
}
