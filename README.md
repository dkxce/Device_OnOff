# Device_OnOff

Device On/Off switcher (TouchPad, Mouse, Flash Driver..)

Маленькая программка для включения и выключения устройств (например, точпада).   
Написана специально для HP ноутбука, чтобы влючать и отключать на нем TouchPad.   

При первом запуске спрашивает **DeviceID** устройства, который необходимо найти    
в **диспетчере устройств** (**DeviceID** или **Путь к экземпляру устройства**).

Затем сохраняет данные в реестр и при каждом запуске переключает устройство с **вкл** на **выкл**    
и обратно, записывая статус изменения в реестр.

**Чтобы изменить устройство**, необходимо пока программа работает напечатать в окне консоли   
любой текст, тогда откроется окно ввода/выбора DeviceID.   
