
# Compile stuff
./update.py

# Backup crontab
crontab -l > crontab.old
crontab -l > crontab.tmp

echo "5  *  *  *  *   cd `pwd` && ./update.py" >> crontab.tmp
echo "11  *  *  *  *   cd `pwd`/bin && ./Watchtower.Crawler" >> crontab.tmp

echo "DIFFERENCES BETWEEN OLD AND NEW CRONTAB:"
diff crontab.old crontab.tmp

echo "REPLACE THE OLD CRONTAB WITH THE CONTENTS OF crontab.tmp:"
echo "$ crontab - ./crontab.tmp && rm ./crontab.tmp"
